// Decompiled with JetBrains decompiler
// Type: RelicService.View.ProfileEditForm
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Data.Event;
using RelicService.Service;
using RelicService.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  internal class ProfileEditForm : Form
  {
    private readonly AvatarService _avatarService;
    private readonly ResourceManager _resourceManager;
    private readonly EventManager _eventManager;
    private readonly SqliteContext _dbContext;
    private readonly Dictionary<uint, Image> _avatarImageMap = new Dictionary<uint, Image>();
    private readonly HashSet<uint> _avatarImageRef = new HashSet<uint>();
    private 
    #nullable disable
    IContainer components;
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

    public 
    #nullable enable
    DbRelicProfile? RelicProfile { get; set; }

    public ProfileEditForm(
      AvatarService avatarService,
      ResourceManager resourceManager,
      EventManager eventManager,
      SqliteContext dbContext)
    {
      this.InitializeComponent();
      this._avatarService = avatarService;
      this._resourceManager = resourceManager;
      this._eventManager = eventManager;
      this._dbContext = dbContext;
    }

    private async void ProfileEditForm_Load(object sender, EventArgs e)
    {
      ProfileEditForm profileEditForm = this;
      profileEditForm.SetupBindings();
      if (profileEditForm.RelicProfile == null)
        return;
      profileEditForm.Text = "编辑配置 - " + profileEditForm.RelicProfile.ProfileName;
      if (profileEditForm.RelicProfile.WithScene.Count > 0)
        profileEditForm.tbSceneIds.Text = Enumerable.Aggregate<string>(Enumerable.Select<uint, string>((IEnumerable<uint>) profileEditForm.RelicProfile.WithScene, (Func<uint, string>) (s => s.ToString())), (Func<string, string, string>) ((a, b) => a + "," + b));
      await profileEditForm.PopulateTeamList();
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
      ProfileEditForm profileEditForm = this;
      int num = await profileEditForm._dbContext.SaveChangesAsync();
      profileEditForm._eventManager.FireEventAsync(EventId.EvtProfileRefresh);
      profileEditForm.Close();
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
      ProfileEditForm profileEditForm = this;
      if (MessageBox.Show("确定删除？", "删除配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No || profileEditForm.RelicProfile == null)
        return;
      profileEditForm._dbContext.RelicProfiles.Remove(profileEditForm.RelicProfile);
      int num = await profileEditForm._dbContext.SaveChangesAsync();
      profileEditForm._eventManager.FireEventAsync(EventId.EvtProfileRefresh);
      profileEditForm.Close();
    }

    private async void btnAddTeam_Click(object sender, EventArgs e)
    {
      if (this.RelicProfile == null)
        ;
      else
      {
        AvatarSelectionForm requiredService = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
        requiredService.IsMultiSelect = true;
        requiredService.MultiSelectLimit = 3U;
        if (requiredService.ShowDialog() == DialogResult.Cancel)
          ;
        else
        {
          List<uint> avatarIds = Enumerable.ToList<uint>((IEnumerable<uint>) Enumerable.Order<uint>(Enumerable.Select<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) Enumerable.ToList<DbUserAvatar>(Enumerable.Where<DbUserAvatar>((IEnumerable<DbUserAvatar>) await Task.WhenAll<DbUserAvatar>(Enumerable.Select<ulong, Task<DbUserAvatar>>((IEnumerable<ulong>) requiredService.SelectedAvatarGuids, (Func<ulong, Task<DbUserAvatar>>) (async g => await this._avatarService.GetUserAvatarByGuid(g)))), (Func<DbUserAvatar, bool>) (u => u != null))), (Func<DbUserAvatar, uint>) (ua => ua.Avatar.AvatarId))));
          if (Enumerable.Any<DbRelicProfileTeamContext>((IEnumerable<DbRelicProfileTeamContext>) this.RelicProfile.TeamContexts, (Func<DbRelicProfileTeamContext, bool>) (tc => tc.AvatarIds.Count == avatarIds.Count && Enumerable.All<uint>((IEnumerable<uint>) tc.AvatarIds, new Func<uint, bool>(avatarIds.Contains)))))
          {
            int num = (int) MessageBox.Show("队伍已存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          }
          else
          {
            this.RelicProfile.TeamContexts.Add(new DbRelicProfileTeamContext()
            {
              AvatarIds = avatarIds,
              Profile = this.RelicProfile
            });
            await this.PopulateTeamList();
          }
        }
      }
    }

    private void ProfileEditForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Enumerable.ToList<uint>((IEnumerable<uint>) this._avatarImageRef).ForEach(new Action<uint>(this._resourceManager.FreeAvatarImage));
    }

    private async void AvatarTeamItem_OnDeleteClick(AvatarTeamItem panel, int controlIndex)
    {
      DbRelicProfile relicProfile = this.RelicProfile;
      DbRelicProfileTeamContext profileTeamContext = relicProfile != null ? Enumerable.ElementAtOrDefault<DbRelicProfileTeamContext>((IEnumerable<DbRelicProfileTeamContext>) relicProfile.TeamContexts, controlIndex) : (DbRelicProfileTeamContext) null;
      if (profileTeamContext == null)
        return;
      this.RelicProfile?.TeamContexts.Remove(profileTeamContext);
      this.flpTeams.Controls.Remove((Control) panel);
      panel.Dispose();
      await this.PopulateTeamList();
    }

    private void SetupBindings()
    {
      this.tbProfileName.DataBindings.Add("Text", (object) this.RelicProfile, "ProfileName");
      this.cbAutoEquip.DataBindings.Add("Checked", (object) this.RelicProfile, "AutoEquip");
      Binding binding = new Binding("Text", (object) this.RelicProfile, "WithScene");
      binding.Format += (ConvertEventHandler) ((s, e) =>
      {
        if (!(e.Value is List<uint> uintList2))
          return;
        e.Value = (object) string.Join<uint>(",", (IEnumerable<uint>) uintList2);
      });
      binding.Parse += (ConvertEventHandler) ((s, e) =>
      {
        if (!(e.Value is string str2))
          return;
        uint num;
        List<uint> list = Enumerable.ToList<uint>(Enumerable.Select<(bool, uint), uint>(Enumerable.Where<(bool, uint)>(Enumerable.Select<string, (bool, uint)>((IEnumerable<string>) str2.Replace(" ", "").Split(",", (StringSplitOptions) 1), (Func<string, (bool, uint)>) (str => (uint.TryParse(str, ref num), num))), (Func<(bool, uint), bool>) (p => p.success)), (Func<(bool, uint), uint>) (p => p.result)));
        e.Value = (object) list;
      });
      this.tbSceneIds.DataBindings.Add(binding);
      this.labelSceneId.DataBindings.Add("Enabled", (object) this.cbAutoEquip, "Checked");
      this.tbSceneIds.DataBindings.Add("Enabled", (object) this.cbAutoEquip, "Checked");
      this.labelTeam.DataBindings.Add("Enabled", (object) this.cbAutoEquip, "Checked");
      this.flpTeams.DataBindings.Add("Enabled", (object) this.cbAutoEquip, "Checked");
      this.btnAddTeam.DataBindings.Add("Enabled", (object) this.cbAutoEquip, "Checked");
    }

    private async Task PopulateTeamList()
    {
      ProfileEditForm profileEditForm = this;
      Enumerable.ToList<AvatarTeamItem>(Enumerable.OfType<AvatarTeamItem>((IEnumerable) profileEditForm.flpTeams.Controls)).ForEach((Action<AvatarTeamItem>) (c => c.Dispose()));
      profileEditForm.flpTeams.Controls.Clear();
      if (profileEditForm.RelicProfile == null)
        return;
      foreach (DbRelicProfileTeamContext teamContext in (IEnumerable<DbRelicProfileTeamContext>) profileEditForm.RelicProfile.TeamContexts)
      {
        AvatarTeamItem panel = new AvatarTeamItem();
        panel.ControlIndex = profileEditForm.flpTeams.Controls.Count;
        panel.OnDeleteCallback = new Action<AvatarTeamItem, int>(profileEditForm.AvatarTeamItem_OnDeleteClick);
        List<Image> images = new List<Image>();
        foreach (uint avatarId in teamContext.AvatarIds)
        {
          Image image;
          if (!profileEditForm._avatarImageMap.TryGetValue(avatarId, ref image))
          {
            DbAvatar avatarMeta = await profileEditForm._avatarService.GetAvatarMetadata(avatarId);
            if (avatarMeta != null)
            {
              image = await profileEditForm._resourceManager.GetAvatarImage(avatarMeta.AvatarId);
              if (image != null)
              {
                image = Utils.ResizeImage(image, 80, 80);
                profileEditForm._avatarImageRef.Add(avatarMeta.AvatarId);
                profileEditForm._avatarImageMap.Add(avatarId, image);
                avatarMeta = (DbAvatar) null;
              }
              else
                continue;
            }
            else
              continue;
          }
          images.Add(image);
        }
        panel.AvatarImage1 = Enumerable.ElementAtOrDefault<Image>((IEnumerable<Image>) images, 0);
        panel.AvatarImage2 = Enumerable.ElementAtOrDefault<Image>((IEnumerable<Image>) images, 1);
        panel.AvatarImage3 = Enumerable.ElementAtOrDefault<Image>((IEnumerable<Image>) images, 2);
        profileEditForm.flpTeams.Controls.Add((Control) panel);
        panel = (AvatarTeamItem) null;
        images = (List<Image>) null;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        ((IDisposable) this.components).Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tableLayoutPanel = new TableLayoutPanel();
      this.labelProfileName = new Label();
      this.tbProfileName = new TextBox();
      this.labelSceneId = new Label();
      this.tbSceneIds = new TextBox();
      this.btnSave = new Button();
      this.btnDelete = new Button();
      this.labelTeam = new Label();
      this.btnAddTeam = new Button();
      this.flpTeams = new FlowLayoutPanel();
      this.cbAutoEquip = new CheckBox();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      this.tableLayoutPanel.ColumnCount = 4;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81f));
      this.tableLayoutPanel.Controls.Add((Control) this.labelProfileName, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.tbProfileName, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelSceneId, 0, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.tbSceneIds, 1, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.btnSave, 3, 4);
      this.tableLayoutPanel.Controls.Add((Control) this.btnDelete, 2, 4);
      this.tableLayoutPanel.Controls.Add((Control) this.labelTeam, 0, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.btnAddTeam, 1, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.flpTeams, 1, 3);
      this.tableLayoutPanel.Controls.Add((Control) this.cbAutoEquip, 0, 4);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(5, 5);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 5;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
      this.tableLayoutPanel.Size = new Size(294, 431);
      this.tableLayoutPanel.TabIndex = 0;
      this.labelProfileName.AutoSize = true;
      this.labelProfileName.Location = new Point(3, 7);
      this.labelProfileName.Margin = new Padding(3, 7, 3, 3);
      this.labelProfileName.Name = "labelProfileName";
      this.labelProfileName.Size = new Size(46, 15);
      this.labelProfileName.TabIndex = 0;
      this.labelProfileName.Text = "配置名";
      this.tableLayoutPanel.SetColumnSpan((Control) this.tbProfileName, 3);
      this.tbProfileName.Dock = DockStyle.Fill;
      this.tbProfileName.Location = new Point(55, 3);
      this.tbProfileName.Name = "tbProfileName";
      this.tbProfileName.Size = new Size(236, 23);
      this.tbProfileName.TabIndex = 1;
      this.labelSceneId.AutoSize = true;
      this.labelSceneId.Location = new Point(3, 36);
      this.labelSceneId.Margin = new Padding(3, 7, 3, 3);
      this.labelSceneId.Name = "labelSceneId";
      this.labelSceneId.Size = new Size(33, 15);
      this.labelSceneId.TabIndex = 2;
      this.labelSceneId.Text = "场景";
      this.tableLayoutPanel.SetColumnSpan((Control) this.tbSceneIds, 3);
      this.tbSceneIds.Dock = DockStyle.Fill;
      this.tbSceneIds.Location = new Point(55, 32);
      this.tbSceneIds.Name = "tbSceneIds";
      this.tbSceneIds.Size = new Size(236, 23);
      this.tbSceneIds.TabIndex = 3;
      this.btnSave.Location = new Point(216, 404);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(75, 23);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "保存";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.btnDelete.Dock = DockStyle.Right;
      this.btnDelete.Location = new Point(135, 404);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new Size(75, 24);
      this.btnDelete.TabIndex = 6;
      this.btnDelete.Text = "删除";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
      this.labelTeam.AutoSize = true;
      this.labelTeam.Location = new Point(3, 65);
      this.labelTeam.Margin = new Padding(3, 7, 3, 3);
      this.labelTeam.Name = "labelTeam";
      this.labelTeam.Size = new Size(33, 15);
      this.labelTeam.TabIndex = 4;
      this.labelTeam.Text = "队伍";
      this.tableLayoutPanel.SetColumnSpan((Control) this.btnAddTeam, 3);
      this.btnAddTeam.Dock = DockStyle.Fill;
      this.btnAddTeam.Location = new Point(55, 61);
      this.btnAddTeam.Name = "btnAddTeam";
      this.btnAddTeam.Size = new Size(236, 23);
      this.btnAddTeam.TabIndex = 7;
      this.btnAddTeam.Text = "添加队伍";
      this.btnAddTeam.UseVisualStyleBackColor = true;
      this.btnAddTeam.Click += new EventHandler(this.btnAddTeam_Click);
      this.flpTeams.AutoScroll = true;
      this.flpTeams.BackColor = SystemColors.Window;
      this.flpTeams.BorderStyle = BorderStyle.FixedSingle;
      this.tableLayoutPanel.SetColumnSpan((Control) this.flpTeams, 3);
      this.flpTeams.Dock = DockStyle.Fill;
      this.flpTeams.FlowDirection = FlowDirection.TopDown;
      this.flpTeams.Location = new Point(55, 90);
      this.flpTeams.Name = "flpTeams";
      this.flpTeams.Size = new Size(236, 308);
      this.flpTeams.TabIndex = 8;
      this.flpTeams.WrapContents = false;
      this.cbAutoEquip.AutoSize = true;
      this.tableLayoutPanel.SetColumnSpan((Control) this.cbAutoEquip, 2);
      this.cbAutoEquip.Location = new Point(3, 407);
      this.cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
      this.cbAutoEquip.Name = "cbAutoEquip";
      this.cbAutoEquip.Size = new Size(78, 19);
      this.cbAutoEquip.TabIndex = 9;
      this.cbAutoEquip.Text = "自动装备";
      this.cbAutoEquip.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(304, 441);
      this.Controls.Add((Control) this.tableLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ProfileEditForm);
      this.Padding = new Padding(5);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "编辑配置：";
      this.FormClosed += new FormClosedEventHandler(this.ProfileEditForm_FormClosed);
      this.Load += new EventHandler(this.ProfileEditForm_Load);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
