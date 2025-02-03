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
			_gameMessageService.EnqueueMessage($"[{avatarName}] is equipped with [{RelicProfile. ProfileName}] Presets");
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
				_gameMessageService.EnqueueMessage($"[{avatarName}] is equipped with [{profileAvatarName}->{RelicProfile.ProfileName}] Presets");
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
		this.components = new System.ComponentModel.Container();
		this.groupBox = new System.Windows.Forms.GroupBox();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.labelHead = new System.Windows.Forms.Label();
		this.labelGoblet = new System.Windows.Forms.Label();
		this.labelSand = new System.Windows.Forms.Label();
		this.labelFeather = new System.Windows.Forms.Label();
		this.pbHead = new System.Windows.Forms.PictureBox();
		this.pbGoblet = new System.Windows.Forms.PictureBox();
		this.pbSand = new System.Windows.Forms.PictureBox();
		this.pbFeather = new System.Windows.Forms.PictureBox();
		this.pbFlower = new System.Windows.Forms.PictureBox();
		this.labelFlower = new System.Windows.Forms.Label();
		this.btnEdit = new System.Windows.Forms.Button();
		this.btnQuickEquip = new System.Windows.Forms.Button();
		this.btnEquipTo = new System.Windows.Forms.Button();
		this.labelConflict = new System.Windows.Forms.Label();
		this.toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.groupBox.SuspendLayout();
		this.tableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbHead).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbGoblet).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbSand).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbFeather).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbFlower).BeginInit();
		base.SuspendLayout();
		this.groupBox.Controls.Add(this.tableLayoutPanel);
		this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBox.Location = new System.Drawing.Point(0, 0);
		this.groupBox.Name = "groupBox";
		this.groupBox.Padding = new System.Windows.Forms.Padding(5, 3, 5, 5);
		this.groupBox.Size = new System.Drawing.Size(875, 170);
		this.groupBox.TabIndex = 0;
		this.groupBox.TabStop = false;
		this.groupBox.Text = "<Preset Name>";
		this.tableLayoutPanel.ColumnCount = 10;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.Controls.Add(this.labelHead, 9, 0);
		this.tableLayoutPanel.Controls.Add(this.labelGoblet, 7, 0);
		this.tableLayoutPanel.Controls.Add(this.labelSand, 5, 0);
		this.tableLayoutPanel.Controls.Add(this.labelFeather, 3, 0);
		this.tableLayoutPanel.Controls.Add(this.pbHead, 8, 0);
		this.tableLayoutPanel.Controls.Add(this.pbGoblet, 6, 0);
		this.tableLayoutPanel.Controls.Add(this.pbSand, 4, 0);
		this.tableLayoutPanel.Controls.Add(this.pbFeather, 2, 0);
		this.tableLayoutPanel.Controls.Add(this.pbFlower, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.labelFlower, 1, 0);
		this.tableLayoutPanel.Controls.Add(this.btnEdit, 9, 2);
		this.tableLayoutPanel.Controls.Add(this.btnQuickEquip, 8, 2);
		this.tableLayoutPanel.Controls.Add(this.btnEquipTo, 7, 2);
		this.tableLayoutPanel.Controls.Add(this.labelConflict, 0, 2);
		this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel.Location = new System.Drawing.Point(5, 19);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 3;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(865, 146);
		this.tableLayoutPanel.TabIndex = 0;
		this.labelHead.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelHead.Location = new System.Drawing.Point(777, 3);
		this.labelHead.Margin = new System.Windows.Forms.Padding(3);
		this.labelHead.Name = "labelHead";
		this.tableLayoutPanel.SetRowSpan(this.labelHead, 2);
		this.labelHead.Size = new System.Drawing.Size(85, 110);
		this.labelHead.TabIndex = 9;
		this.labelHead.Text = "label1";
		this.labelGoblet.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelGoblet.Location = new System.Drawing.Point(605, 3);
		this.labelGoblet.Margin = new System.Windows.Forms.Padding(3);
		this.labelGoblet.Name = "labelGoblet";
		this.tableLayoutPanel.SetRowSpan(this.labelGoblet, 2);
		this.labelGoblet.Size = new System.Drawing.Size(80, 110);
		this.labelGoblet.TabIndex = 8;
		this.labelGoblet.Text = "label1";
		this.labelSand.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelSand.Location = new System.Drawing.Point(433, 3);
		this.labelSand.Margin = new System.Windows.Forms.Padding(3);
		this.labelSand.Name = "labelSand";
		this.tableLayoutPanel.SetRowSpan(this.labelSand, 2);
		this.labelSand.Size = new System.Drawing.Size(80, 110);
		this.labelSand.TabIndex = 7;
		this.labelSand.Text = "label1";
		this.labelFeather.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelFeather.Location = new System.Drawing.Point(261, 3);
		this.labelFeather.Margin = new System.Windows.Forms.Padding(3);
		this.labelFeather.Name = "labelFeather";
		this.tableLayoutPanel.SetRowSpan(this.labelFeather, 2);
		this.labelFeather.Size = new System.Drawing.Size(80, 110);
		this.labelFeather.TabIndex = 6;
		this.labelFeather.Text = "label1";
		this.pbHead.Image = RelicService.Properties.Resources.dot_green;
		this.pbHead.Location = new System.Drawing.Point(691, 3);
		this.pbHead.MaximumSize = new System.Drawing.Size(80, 80);
		this.pbHead.MinimumSize = new System.Drawing.Size(60, 60);
		this.pbHead.Name = "pbHead";
		this.pbHead.Size = new System.Drawing.Size(80, 80);
		this.pbHead.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbHead.TabIndex = 5;
		this.pbHead.TabStop = false;
		this.pbHead.Resize += new System.EventHandler(PictureBox_Resize);
		this.pbGoblet.Image = RelicService.Properties.Resources.dot_green;
		this.pbGoblet.Location = new System.Drawing.Point(519, 3);
		this.pbGoblet.MaximumSize = new System.Drawing.Size(80, 80);
		this.pbGoblet.MinimumSize = new System.Drawing.Size(60, 60);
		this.pbGoblet.Name = "pbGoblet";
		this.pbGoblet.Size = new System.Drawing.Size(80, 80);
		this.pbGoblet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbGoblet.TabIndex = 4;
		this.pbGoblet.TabStop = false;
		this.pbGoblet.Resize += new System.EventHandler(PictureBox_Resize);
		this.pbSand.Image = RelicService.Properties.Resources.dot_green;
		this.pbSand.Location = new System.Drawing.Point(347, 3);
		this.pbSand.MaximumSize = new System.Drawing.Size(80, 80);
		this.pbSand.MinimumSize = new System.Drawing.Size(60, 60);
		this.pbSand.Name = "pbSand";
		this.pbSand.Size = new System.Drawing.Size(80, 80);
		this.pbSand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbSand.TabIndex = 3;
		this.pbSand.TabStop = false;
		this.pbSand.Resize += new System.EventHandler(PictureBox_Resize);
		this.pbFeather.Image = RelicService.Properties.Resources.dot_green;
		this.pbFeather.Location = new System.Drawing.Point(175, 3);
		this.pbFeather.MaximumSize = new System.Drawing.Size(80, 80);
		this.pbFeather.MinimumSize = new System.Drawing.Size(60, 60);
		this.pbFeather.Name = "pbFeather";
		this.pbFeather.Size = new System.Drawing.Size(80, 80);
		this.pbFeather.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbFeather.TabIndex = 2;
		this.pbFeather.TabStop = false;
		this.pbFeather.Resize += new System.EventHandler(PictureBox_Resize);
		this.pbFlower.Image = RelicService.Properties.Resources.dot_green;
		this.pbFlower.Location = new System.Drawing.Point(3, 3);
		this.pbFlower.MaximumSize = new System.Drawing.Size(80, 80);
		this.pbFlower.MinimumSize = new System.Drawing.Size(60, 60);
		this.pbFlower.Name = "pbFlower";
		this.pbFlower.Size = new System.Drawing.Size(80, 80);
		this.pbFlower.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbFlower.TabIndex = 0;
		this.pbFlower.TabStop = false;
		this.pbFlower.Resize += new System.EventHandler(PictureBox_Resize);
		this.labelFlower.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelFlower.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelFlower.Location = new System.Drawing.Point(89, 3);
		this.labelFlower.Margin = new System.Windows.Forms.Padding(3);
		this.labelFlower.Name = "labelFlower";
		this.tableLayoutPanel.SetRowSpan(this.labelFlower, 2);
		this.labelFlower.Size = new System.Drawing.Size(80, 110);
		this.labelFlower.TabIndex = 1;
		this.labelFlower.Text = "label1";
		this.btnEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btnEdit.Location = new System.Drawing.Point(777, 119);
		this.btnEdit.Name = "btnEdit";
		this.btnEdit.Size = new System.Drawing.Size(85, 24);
		this.btnEdit.TabIndex = 10;
		this.btnEdit.Text = "Edit";
		this.toolTip.SetToolTip(this.btnEdit, "Change or delete settings");
		this.btnEdit.UseVisualStyleBackColor = true;
		this.btnEdit.Click += new System.EventHandler(btnEdit_Click);
		this.btnQuickEquip.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btnQuickEquip.Location = new System.Drawing.Point(691, 119);
		this.btnQuickEquip.Name = "btnQuickEquip";
		this.btnQuickEquip.Size = new System.Drawing.Size(80, 24);
		this.btnQuickEquip.TabIndex = 11;
		this.btnQuickEquip.Text = "Fast Equip";
		this.toolTip.SetToolTip(this.btnQuickEquip, "Equip to Current Character");
		this.btnQuickEquip.UseVisualStyleBackColor = true;
		this.btnQuickEquip.Click += new System.EventHandler(btnQuickEquip_Click);
		this.btnEquipTo.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btnEquipTo.Location = new System.Drawing.Point(605, 119);
		this.btnEquipTo.Name = "btnEquipTo";
		this.btnEquipTo.Size = new System.Drawing.Size(80, 24);
		this.btnEquipTo.TabIndex = 12;
		this.btnEquipTo.Text = "Equip To...";
		this.toolTip.SetToolTip(this.btnEquipTo, "Equip for other Character");
		this.btnEquipTo.UseVisualStyleBackColor = true;
		this.btnEquipTo.Click += new System.EventHandler(btnEquipTo_Click);
		this.labelConflict.AutoSize = true;
		this.tableLayoutPanel.SetColumnSpan(this.labelConflict, 6);
		this.labelConflict.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelConflict.ForeColor = System.Drawing.Color.Red;
		this.labelConflict.Location = new System.Drawing.Point(3, 123);
		this.labelConflict.Margin = new System.Windows.Forms.Padding(3, 7, 5, 5);
		this.labelConflict.Name = "labelConflict";
		this.labelConflict.Size = new System.Drawing.Size(35, 15);
		this.labelConflict.TabIndex = 13;
		this.labelConflict.Text = "Conflict";
		this.toolTip.AutomaticDelay = 0;
		this.toolTip.AutoPopDelay = 5000;
		this.toolTip.InitialDelay = 0;
		this.toolTip.ReshowDelay = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.groupBox);
		base.Name = "ProfileDetails";
		base.Size = new System.Drawing.Size(875, 170);
		this.groupBox.ResumeLayout(false);
		this.tableLayoutPanel.ResumeLayout(false);
		this.tableLayoutPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pbHead).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbGoblet).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbSand).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbFeather).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbFlower).EndInit();
		base.ResumeLayout(false);
	}
}
