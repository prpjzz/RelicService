using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Properties;
using RelicService.Service;

namespace RelicService.View;

internal class ProfileDetails : UserControl
{
	public static readonly float WidthToHeightScale = 34f / 175f;

	private EquipService _equipService;

	private AvatarService _avatarService;

	private GameMessageService _gameMessageService;

	private readonly object _lock = new object();

	private IContainer components;

	private GroupBox groupBox;

	private TableLayoutPanel tableLayoutPanel;

	private PictureBox pbFlower;

	private Label labelFlower;

	private Label labelHead;

	private Label labelGoblet;

	private Label labelSand;

	private Label labelFeather;

	private PictureBox pbHead;

	private PictureBox pbGoblet;

	private PictureBox pbSand;

	private PictureBox pbFeather;

	private Button btnEdit;

	private Button btnQuickEquip;

	private Button btnEquipTo;

	private ToolTip toolTip;

	private Label labelConflict;

	public string ProfileName
	{
		get
		{
			return groupBox.Text;
		}
		set
		{
			groupBox.Text = value;
		}
	}

	public Image FlowerImage
	{
		set
		{
			pbFlower.Image = value;
		}
	}

	public Image FeatherImage
	{
		set
		{
			pbFeather.Image = value;
		}
	}

	public Image SandImage
	{
		set
		{
			pbSand.Image = value;
		}
	}

	public Image GobletImage
	{
		set
		{
			pbGoblet.Image = value;
		}
	}

	public Image HeadImage
	{
		set
		{
			pbHead.Image = value;
		}
	}

	public string FlowerLabel
	{
		set
		{
			labelFlower.Text = value;
		}
	}

	public string FeatherLabel
	{
		set
		{
			labelFeather.Text = value;
		}
	}

	public string SandLabel
	{
		set
		{
			labelSand.Text = value;
		}
	}

	public string GobletLabel
	{
		set
		{
			labelGoblet.Text = value;
		}
	}

	public string HeadLabel
	{
		set
		{
			labelHead.Text = value;
		}
	}

	public string ConflicLabel
	{
		set
		{
			labelConflict.Text = value;
		}
	}

	public DbRelicProfile? RelicProfile { get; set; }

	public ProfileDetails()
	{
	}

	public ProfileDetails(EquipService equipService, AvatarService avatarService, GameMessageService gameMessageService)
	{
		InitializeComponent();
		_equipService = equipService;
		_avatarService = avatarService;
		_gameMessageService = gameMessageService;
		FlowerImage = null;
		FeatherImage = null;
		SandImage = null;
		GobletImage = null;
		HeadImage = null;
		FlowerLabel = string.Empty;
		FeatherLabel = string.Empty;
		SandLabel = string.Empty;
		GobletLabel = string.Empty;
		HeadLabel = string.Empty;
	}

	private async void btnQuickEquip_Click(object sender, EventArgs e)
	{
		if (RelicProfile != null)
		{
			string avatarName = RelicProfile.UserAvatar.Avatar.Name;
			ulong avatarGuid = RelicProfile.AvatarGuid;
			await EquipToAvatar(avatarGuid);
			_gameMessageService.EnqueueMessage($"已为 [{avatarName}] 装备 [{RelicProfile.ProfileName}] 预设");
		}
	}

	private async void btnEquipTo_Click(object sender, EventArgs e)
	{
		if (RelicProfile == null)
		{
			return;
		}
		AvatarSelectionForm requiredService = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
		if (requiredService.ShowDialog() != DialogResult.Cancel)
		{
			ulong selectedAvatarGuid = requiredService.SelectedAvatarGuid;
			DbUserAvatar dbUserAvatar = await _avatarService.GetUserAvatarByGuid(selectedAvatarGuid);
			if (dbUserAvatar != null)
			{
				string profileAvatarName = RelicProfile.UserAvatar.Avatar.Name;
				string avatarName = dbUserAvatar.Avatar.Name;
				await EquipToAvatar(selectedAvatarGuid);
				_gameMessageService.EnqueueMessage($"已为 [{avatarName}] 装备 [{profileAvatarName}->{RelicProfile.ProfileName}] 预设");
			}
		}
	}

	private void btnEdit_Click(object sender, EventArgs e)
	{
		ProfileEditForm requiredService = Program.ServiceProvider.GetRequiredService<ProfileEditForm>();
		requiredService.RelicProfile = RelicProfile;
		requiredService.ShowDialog();
	}

	private void PictureBox_Resize(object sender, EventArgs e)
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(_lock, ref lockTaken);
			if (lockTaken)
			{
				PictureBox pictureBox = sender as PictureBox;
				Size size = pictureBox.Size;
				size.Height = pictureBox.Size.Width;
				pictureBox.Size = size;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(_lock);
			}
		}
	}

	private async Task EquipToAvatar(ulong avatarGuid)
	{
		if (RelicProfile?.RelicItems == null)
		{
			return;
		}
		foreach (DbRelicItem relicItem in RelicProfile.RelicItems)
		{
			ulong guid = relicItem.Guid;
			await _equipService.WearEquip(avatarGuid, guid);
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
        components = new Container();
        groupBox = new GroupBox();
        tableLayoutPanel = new TableLayoutPanel();
        labelHead = new Label();
        labelGoblet = new Label();
        labelSand = new Label();
        labelFeather = new Label();
        pbHead = new PictureBox();
        pbGoblet = new PictureBox();
        pbSand = new PictureBox();
        pbFeather = new PictureBox();
        pbFlower = new PictureBox();
        labelFlower = new Label();
        btnEdit = new Button();
        btnQuickEquip = new Button();
        btnEquipTo = new Button();
        labelConflict = new Label();
        toolTip = new ToolTip(components);
        groupBox.SuspendLayout();
        tableLayoutPanel.SuspendLayout();
        ((ISupportInitialize)pbHead).BeginInit();
        ((ISupportInitialize)pbGoblet).BeginInit();
        ((ISupportInitialize)pbSand).BeginInit();
        ((ISupportInitialize)pbFeather).BeginInit();
        ((ISupportInitialize)pbFlower).BeginInit();
        SuspendLayout();
        // 
        // groupBox
        // 
        groupBox.Controls.Add(tableLayoutPanel);
        groupBox.Dock = DockStyle.Fill;
        groupBox.Location = new Point(0, 0);
        groupBox.Name = "groupBox";
        groupBox.Padding = new Padding(5, 3, 5, 5);
        groupBox.Size = new Size(875, 217);
        groupBox.TabIndex = 0;
        groupBox.TabStop = false;
        groupBox.Text = "<预设名>";
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.ColumnCount = 10;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel.Controls.Add(labelHead, 9, 0);
        tableLayoutPanel.Controls.Add(labelGoblet, 7, 0);
        tableLayoutPanel.Controls.Add(labelSand, 5, 0);
        tableLayoutPanel.Controls.Add(labelFeather, 3, 0);
        tableLayoutPanel.Controls.Add(pbHead, 8, 0);
        tableLayoutPanel.Controls.Add(pbGoblet, 6, 0);
        tableLayoutPanel.Controls.Add(pbSand, 4, 0);
        tableLayoutPanel.Controls.Add(pbFeather, 2, 0);
        tableLayoutPanel.Controls.Add(pbFlower, 0, 0);
        tableLayoutPanel.Controls.Add(labelFlower, 1, 0);
        tableLayoutPanel.Controls.Add(btnEdit, 9, 2);
        tableLayoutPanel.Controls.Add(btnQuickEquip, 8, 2);
        tableLayoutPanel.Controls.Add(btnEquipTo, 7, 2);
        tableLayoutPanel.Controls.Add(labelConflict, 0, 2);
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.Location = new Point(5, 19);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 72.8971939F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 27.1028042F));
        tableLayoutPanel.Size = new Size(865, 193);
        tableLayoutPanel.TabIndex = 0;
        // 
        // labelHead
        // 
        labelHead.Dock = DockStyle.Fill;
        labelHead.Location = new Point(777, 3);
        labelHead.Margin = new Padding(3);
        labelHead.Name = "labelHead";
        tableLayoutPanel.SetRowSpan(labelHead, 2);
        labelHead.Size = new Size(85, 157);
        labelHead.TabIndex = 9;
        labelHead.Text = "label1";
        // 
        // labelGoblet
        // 
        labelGoblet.Dock = DockStyle.Fill;
        labelGoblet.Location = new Point(605, 3);
        labelGoblet.Margin = new Padding(3);
        labelGoblet.Name = "labelGoblet";
        tableLayoutPanel.SetRowSpan(labelGoblet, 2);
        labelGoblet.Size = new Size(80, 157);
        labelGoblet.TabIndex = 8;
        labelGoblet.Text = "label1";
        // 
        // labelSand
        // 
        labelSand.Dock = DockStyle.Fill;
        labelSand.Location = new Point(433, 3);
        labelSand.Margin = new Padding(3);
        labelSand.Name = "labelSand";
        tableLayoutPanel.SetRowSpan(labelSand, 2);
        labelSand.Size = new Size(80, 157);
        labelSand.TabIndex = 7;
        labelSand.Text = "label1";
        // 
        // labelFeather
        // 
        labelFeather.Dock = DockStyle.Fill;
        labelFeather.Location = new Point(261, 3);
        labelFeather.Margin = new Padding(3);
        labelFeather.Name = "labelFeather";
        tableLayoutPanel.SetRowSpan(labelFeather, 2);
        labelFeather.Size = new Size(80, 157);
        labelFeather.TabIndex = 6;
        labelFeather.Text = "label1";
        // 
        // pbHead
        // 
        pbHead.Location = new Point(691, 3);
        pbHead.MaximumSize = new Size(80, 80);
        pbHead.MinimumSize = new Size(60, 60);
        pbHead.Name = "pbHead";
        pbHead.Size = new Size(80, 80);
        pbHead.SizeMode = PictureBoxSizeMode.StretchImage;
        pbHead.TabIndex = 5;
        pbHead.TabStop = false;
        pbHead.Resize += PictureBox_Resize;
        // 
        // pbGoblet
        // 
        pbGoblet.Location = new Point(519, 3);
        pbGoblet.MaximumSize = new Size(80, 80);
        pbGoblet.MinimumSize = new Size(60, 60);
        pbGoblet.Name = "pbGoblet";
        pbGoblet.Size = new Size(80, 80);
        pbGoblet.SizeMode = PictureBoxSizeMode.StretchImage;
        pbGoblet.TabIndex = 4;
        pbGoblet.TabStop = false;
        pbGoblet.Resize += PictureBox_Resize;
        // 
        // pbSand
        // 
        pbSand.Location = new Point(347, 3);
        pbSand.MaximumSize = new Size(80, 80);
        pbSand.MinimumSize = new Size(60, 60);
        pbSand.Name = "pbSand";
        pbSand.Size = new Size(80, 80);
        pbSand.SizeMode = PictureBoxSizeMode.StretchImage;
        pbSand.TabIndex = 3;
        pbSand.TabStop = false;
        pbSand.Resize += PictureBox_Resize;
        // 
        // pbFeather
        // 
        pbFeather.Location = new Point(175, 3);
        pbFeather.MaximumSize = new Size(80, 80);
        pbFeather.MinimumSize = new Size(60, 60);
        pbFeather.Name = "pbFeather";
        pbFeather.Size = new Size(80, 80);
        pbFeather.SizeMode = PictureBoxSizeMode.StretchImage;
        pbFeather.TabIndex = 2;
        pbFeather.TabStop = false;
        pbFeather.Resize += PictureBox_Resize;
        // 
        // pbFlower
        // 
        pbFlower.Location = new Point(3, 3);
        pbFlower.MaximumSize = new Size(80, 80);
        pbFlower.MinimumSize = new Size(60, 60);
        pbFlower.Name = "pbFlower";
        pbFlower.Size = new Size(80, 80);
        pbFlower.SizeMode = PictureBoxSizeMode.StretchImage;
        pbFlower.TabIndex = 0;
        pbFlower.TabStop = false;
        pbFlower.Resize += PictureBox_Resize;
        // 
        // labelFlower
        // 
        labelFlower.Dock = DockStyle.Fill;
        labelFlower.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        labelFlower.Location = new Point(89, 3);
        labelFlower.Margin = new Padding(3);
        labelFlower.Name = "labelFlower";
        tableLayoutPanel.SetRowSpan(labelFlower, 2);
        labelFlower.Size = new Size(80, 157);
        labelFlower.TabIndex = 1;
        labelFlower.Text = "label1";
        // 
        // btnEdit
        // 
        btnEdit.Dock = DockStyle.Fill;
        btnEdit.Location = new Point(777, 166);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(85, 24);
        btnEdit.TabIndex = 10;
        btnEdit.Text = "Edit";
        toolTip.SetToolTip(btnEdit, "更改设置或删除");
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnQuickEquip
        // 
        btnQuickEquip.Dock = DockStyle.Fill;
        btnQuickEquip.Location = new Point(691, 166);
        btnQuickEquip.Name = "btnQuickEquip";
        btnQuickEquip.Size = new Size(80, 24);
        btnQuickEquip.TabIndex = 11;
        btnQuickEquip.Text = "Fast Equip";
        toolTip.SetToolTip(btnQuickEquip, "装备到当前角色");
        btnQuickEquip.UseVisualStyleBackColor = true;
        btnQuickEquip.Click += btnQuickEquip_Click;
        // 
        // btnEquipTo
        // 
        btnEquipTo.Dock = DockStyle.Fill;
        btnEquipTo.Location = new Point(605, 166);
        btnEquipTo.Name = "btnEquipTo";
        btnEquipTo.Size = new Size(80, 24);
        btnEquipTo.TabIndex = 12;
        btnEquipTo.Text = "Equip To..";
        toolTip.SetToolTip(btnEquipTo, "装备到其他角色");
        btnEquipTo.UseVisualStyleBackColor = true;
        btnEquipTo.Click += btnEquipTo_Click;
        // 
        // labelConflict
        // 
        labelConflict.AutoSize = true;
        tableLayoutPanel.SetColumnSpan(labelConflict, 6);
        labelConflict.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelConflict.ForeColor = Color.Red;
        labelConflict.Location = new Point(3, 170);
        labelConflict.Margin = new Padding(3, 7, 5, 5);
        labelConflict.Name = "labelConflict";
        labelConflict.Size = new Size(33, 15);
        labelConflict.TabIndex = 13;
        labelConflict.Text = "冲突";
        // 
        // toolTip
        // 
        toolTip.AutomaticDelay = 0;
        toolTip.AutoPopDelay = 5000;
        toolTip.InitialDelay = 0;
        toolTip.ReshowDelay = 0;
        // 
        // ProfileDetails
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(groupBox);
        Name = "ProfileDetails";
        Size = new Size(875, 217);
        groupBox.ResumeLayout(false);
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        ((ISupportInitialize)pbHead).EndInit();
        ((ISupportInitialize)pbGoblet).EndInit();
        ((ISupportInitialize)pbSand).EndInit();
        ((ISupportInitialize)pbFeather).EndInit();
        ((ISupportInitialize)pbFlower).EndInit();
        ResumeLayout(false);
    }
}
