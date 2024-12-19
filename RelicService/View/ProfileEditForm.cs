using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Data.Event;
using RelicService.Service;
using RelicService.Tools;

namespace RelicService.View;

internal class ProfileEditForm : Form
{
	private readonly AvatarService _avatarService;

	private readonly ResourceManager _resourceManager;

	private readonly EventManager _eventManager;

	private readonly SqliteContext _dbContext;

	private readonly Dictionary<uint, Image> _avatarImageMap = new Dictionary<uint, Image>();

	private readonly HashSet<uint> _avatarImageRef = new HashSet<uint>();

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private Label labelProfileName;

	private TextBox tbProfileName;

	private Label labelSceneId;

	private TextBox tbSceneIds;

	private Label labelTeam;

	private Button btnSave;

	private Button btnDelete;

	private Button btnAddTeam;

	private FlowLayoutPanel flpTeams;

	private CheckBox cbAutoEquip;

	public DbRelicProfile? RelicProfile { get; set; }

	public ProfileEditForm(AvatarService avatarService, ResourceManager resourceManager, EventManager eventManager, SqliteContext dbContext)
	{
		InitializeComponent();
		_avatarService = avatarService;
		_resourceManager = resourceManager;
		_eventManager = eventManager;
		_dbContext = dbContext;
	}

	private async void ProfileEditForm_Load(object sender, EventArgs e)
	{
		SetupBindings();
		if (RelicProfile == null)
		{
			return;
		}
		Text = "Edit Configuraton for " + RelicProfile.ProfileName;
		if (RelicProfile.WithScene.Count > 0)
		{
			tbSceneIds.Text = RelicProfile.WithScene.Select((uint s) => s.ToString()).Aggregate((string a, string b) => a + "," + b);
		}
		await PopulateTeamList();
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		await _dbContext.SaveChangesAsync();
		_eventManager.FireEventAsync(EventId.EvtProfileRefresh);
		Close();
	}

	private async void btnDelete_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Confirm Delete？", "Delete Pre-set Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No && RelicProfile != null)
		{
			_dbContext.RelicProfiles.Remove(RelicProfile);
			await _dbContext.SaveChangesAsync();
			_eventManager.FireEventAsync(EventId.EvtProfileRefresh);
			Close();
		}
	}

	private async void btnAddTeam_Click(object sender, EventArgs e)
	{
		if (RelicProfile == null)
		{
			return;
		}
		AvatarSelectionForm requiredService = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
		requiredService.IsMultiSelect = true;
		requiredService.MultiSelectLimit = 3u;
		if (requiredService.ShowDialog() != DialogResult.Cancel)
		{
			List<DbUserAvatar> source = (await Task.WhenAll(requiredService.SelectedAvatarGuids.Select(async (ulong g) => await _avatarService.GetUserAvatarByGuid(g)))).Where((DbUserAvatar u) => u != null).ToList();
			List<uint> avatarIds = source.Select((DbUserAvatar ua) => ua.Avatar.AvatarId).Order().ToList();
			if (RelicProfile.TeamContexts.Any((DbRelicProfileTeamContext tc) => tc.AvatarIds.Count == avatarIds.Count && tc.AvatarIds.All(avatarIds.Contains)))
			{
				MessageBox.Show("队伍已存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			DbRelicProfileTeamContext item = new DbRelicProfileTeamContext
			{
				AvatarIds = avatarIds,
				Profile = RelicProfile
			};
			RelicProfile.TeamContexts.Add(item);
			await PopulateTeamList();
		}
	}

	private void ProfileEditForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		_avatarImageRef.ToList().ForEach(_resourceManager.FreeAvatarImage);
	}

	private async void AvatarTeamItem_OnDeleteClick(AvatarTeamItem panel, int controlIndex)
	{
		DbRelicProfileTeamContext dbRelicProfileTeamContext = RelicProfile?.TeamContexts.ElementAtOrDefault(controlIndex);
		if (dbRelicProfileTeamContext != null)
		{
			RelicProfile?.TeamContexts.Remove(dbRelicProfileTeamContext);
			flpTeams.Controls.Remove(panel);
			panel.Dispose();
			await PopulateTeamList();
		}
	}

	private void SetupBindings()
	{
		tbProfileName.DataBindings.Add("Text", RelicProfile, "ProfileName");
		cbAutoEquip.DataBindings.Add("Checked", RelicProfile, "AutoEquip");
		Binding binding = new Binding("Text", RelicProfile, "WithScene");
		binding.Format += delegate(object? s, ConvertEventArgs e)
		{
			if (e.Value is List<uint> values)
			{
				e.Value = string.Join(",", values);
			}
		};
		binding.Parse += delegate(object? s, ConvertEventArgs e)
		{
			if (e.Value is string text)
			{
				string text2 = text.Replace(" ", "");
				uint result;
				List<uint> value = (from str in text2.Split(",", StringSplitOptions.RemoveEmptyEntries)
					select (success: uint.TryParse(str, out result), result: result) into p
					where p.success
					select p.result).ToList();
				e.Value = value;
			}
		};
		tbSceneIds.DataBindings.Add(binding);
		labelSceneId.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
		tbSceneIds.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
		labelTeam.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
		flpTeams.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
		btnAddTeam.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
	}

	private async Task PopulateTeamList()
	{
		flpTeams.Controls.OfType<AvatarTeamItem>().ToList().ForEach(delegate(AvatarTeamItem c)
		{
			c.Dispose();
		});
		flpTeams.Controls.Clear();
		if (RelicProfile == null)
		{
			return;
		}
		foreach (DbRelicProfileTeamContext teamContext in RelicProfile.TeamContexts)
		{
			AvatarTeamItem panel = new AvatarTeamItem
			{
				ControlIndex = flpTeams.Controls.Count,
				OnDeleteCallback = AvatarTeamItem_OnDeleteClick
			};
			List<Image> images = new List<Image>();
			foreach (uint avatarId in teamContext.AvatarIds)
			{
				if (!_avatarImageMap.TryGetValue(avatarId, out Image value))
				{
					DbAvatar avatarMeta = await _avatarService.GetAvatarMetadata(avatarId);
					if (avatarMeta == null)
					{
						continue;
					}
					value = await _resourceManager.GetAvatarImage(avatarMeta.AvatarId);
					if (value == null)
					{
						continue;
					}
					value = Utils.ResizeImage(value, 80, 80);
					_avatarImageRef.Add(avatarMeta.AvatarId);
					_avatarImageMap.Add(avatarId, value);
				}
				images.Add(value);
			}
			panel.AvatarImage1 = images.ElementAtOrDefault(0);
			panel.AvatarImage2 = images.ElementAtOrDefault(1);
			panel.AvatarImage3 = images.ElementAtOrDefault(2);
			flpTeams.Controls.Add(panel);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

    private void InitializeComponent()
    {
        tableLayoutPanel = new TableLayoutPanel();
        labelProfileName = new Label();
        tbProfileName = new TextBox();
        labelSceneId = new Label();
        tbSceneIds = new TextBox();
        btnSave = new Button();
        btnDelete = new Button();
        labelTeam = new Label();
        btnAddTeam = new Button();
        flpTeams = new FlowLayoutPanel();
        cbAutoEquip = new CheckBox();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.ColumnCount = 4;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81F));
        tableLayoutPanel.Controls.Add(labelProfileName, 0, 0);
        tableLayoutPanel.Controls.Add(tbProfileName, 1, 0);
        tableLayoutPanel.Controls.Add(labelSceneId, 0, 1);
        tableLayoutPanel.Controls.Add(tbSceneIds, 1, 1);
        tableLayoutPanel.Controls.Add(btnSave, 3, 4);
        tableLayoutPanel.Controls.Add(btnDelete, 2, 4);
        tableLayoutPanel.Controls.Add(labelTeam, 0, 2);
        tableLayoutPanel.Controls.Add(btnAddTeam, 1, 2);
        tableLayoutPanel.Controls.Add(flpTeams, 1, 3);
        tableLayoutPanel.Controls.Add(cbAutoEquip, 0, 4);
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.Location = new Point(5, 5);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        tableLayoutPanel.Size = new Size(294, 431);
        tableLayoutPanel.TabIndex = 0;
        // 
        // labelProfileName
        // 
        labelProfileName.AutoSize = true;
        labelProfileName.Location = new Point(3, 7);
        labelProfileName.Margin = new Padding(3, 7, 3, 3);
        labelProfileName.Name = "labelProfileName";
        labelProfileName.Size = new Size(79, 15);
        labelProfileName.TabIndex = 0;
        labelProfileName.Text = "Pre-set Name";
        // 
        // tbProfileName
        // 
        tableLayoutPanel.SetColumnSpan(tbProfileName, 3);
        tbProfileName.Dock = DockStyle.Fill;
        tbProfileName.Location = new Point(88, 3);
        tbProfileName.Name = "tbProfileName";
        tbProfileName.Size = new Size(203, 23);
        tbProfileName.TabIndex = 1;
        // 
        // labelSceneId
        // 
        labelSceneId.AutoSize = true;
        labelSceneId.Location = new Point(3, 36);
        labelSceneId.Margin = new Padding(3, 7, 3, 3);
        labelSceneId.Name = "labelSceneId";
        labelSceneId.Size = new Size(38, 15);
        labelSceneId.TabIndex = 2;
        labelSceneId.Text = "Scene";
        // 
        // tbSceneIds
        // 
        tableLayoutPanel.SetColumnSpan(tbSceneIds, 3);
        tbSceneIds.Dock = DockStyle.Fill;
        tbSceneIds.Location = new Point(88, 32);
        tbSceneIds.Name = "tbSceneIds";
        tbSceneIds.Size = new Size(203, 23);
        tbSceneIds.TabIndex = 3;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(216, 404);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 23);
        btnSave.TabIndex = 5;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnDelete
        // 
        btnDelete.Dock = DockStyle.Right;
        btnDelete.Location = new Point(135, 404);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(75, 24);
        btnDelete.TabIndex = 6;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // labelTeam
        // 
        labelTeam.AutoSize = true;
        labelTeam.Location = new Point(3, 65);
        labelTeam.Margin = new Padding(3, 7, 3, 3);
        labelTeam.Name = "labelTeam";
        labelTeam.Size = new Size(32, 15);
        labelTeam.TabIndex = 4;
        labelTeam.Text = "队伍";
        // 
        // btnAddTeam
        // 
        tableLayoutPanel.SetColumnSpan(btnAddTeam, 3);
        btnAddTeam.Dock = DockStyle.Fill;
        btnAddTeam.Location = new Point(88, 61);
        btnAddTeam.Name = "btnAddTeam";
        btnAddTeam.Size = new Size(203, 23);
        btnAddTeam.TabIndex = 7;
        btnAddTeam.Text = "添加队伍";
        btnAddTeam.UseVisualStyleBackColor = true;
        btnAddTeam.Click += btnAddTeam_Click;
        // 
        // flpTeams
        // 
        flpTeams.AutoScroll = true;
        flpTeams.BackColor = SystemColors.Window;
        flpTeams.BorderStyle = BorderStyle.FixedSingle;
        tableLayoutPanel.SetColumnSpan(flpTeams, 3);
        flpTeams.Dock = DockStyle.Fill;
        flpTeams.FlowDirection = FlowDirection.TopDown;
        flpTeams.Location = new Point(88, 90);
        flpTeams.Name = "flpTeams";
        flpTeams.Size = new Size(203, 308);
        flpTeams.TabIndex = 8;
        flpTeams.WrapContents = false;
        // 
        // cbAutoEquip
        // 
        cbAutoEquip.AutoSize = true;
        tableLayoutPanel.SetColumnSpan(cbAutoEquip, 2);
        cbAutoEquip.Location = new Point(3, 407);
        cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
        cbAutoEquip.Name = "cbAutoEquip";
        cbAutoEquip.Size = new Size(85, 19);
        cbAutoEquip.TabIndex = 9;
        cbAutoEquip.Text = "Auto Equip";
        cbAutoEquip.UseVisualStyleBackColor = true;
        // 
        // ProfileEditForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(304, 441);
        Controls.Add(tableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProfileEditForm";
        Padding = new Padding(5);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Edit Configuration";
        FormClosed += ProfileEditForm_FormClosed;
        Load += ProfileEditForm_Load;
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
