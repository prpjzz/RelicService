// Decompiled with JetBrains decompiler
// Type: RelicService.View.ProfileDetails
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Properties;
using RelicService.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  internal class ProfileDetails : UserControl
  {
    public static readonly float WidthToHeightScale = 0.194285721f;
    private EquipService _equipService;
    private AvatarService _avatarService;
    private GameMessageService _gameMessageService;
    private readonly object _lock = new object();
    private 
    #nullable disable
    IContainer components;
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

    public 
    #nullable enable
    string ProfileName
    {
      get => this.groupBox.Text;
      set => this.groupBox.Text = value;
    }

    public Image FlowerImage
    {
      set => this.pbFlower.Image = value;
    }

    public Image FeatherImage
    {
      set => this.pbFeather.Image = value;
    }

    public Image SandImage
    {
      set => this.pbSand.Image = value;
    }

    public Image GobletImage
    {
      set => this.pbGoblet.Image = value;
    }

    public Image HeadImage
    {
      set => this.pbHead.Image = value;
    }

    public string FlowerLabel
    {
      set => this.labelFlower.Text = value;
    }

    public string FeatherLabel
    {
      set => this.labelFeather.Text = value;
    }

    public string SandLabel
    {
      set => this.labelSand.Text = value;
    }

    public string GobletLabel
    {
      set => this.labelGoblet.Text = value;
    }

    public string HeadLabel
    {
      set => this.labelHead.Text = value;
    }

    public string ConflicLabel
    {
      set => this.labelConflict.Text = value;
    }

    public DbRelicProfile? RelicProfile { get; set; }

    public ProfileDetails()
    {
    }

    public ProfileDetails(
      EquipService equipService,
      AvatarService avatarService,
      GameMessageService gameMessageService)
    {
      this.InitializeComponent();
      this._equipService = equipService;
      this._avatarService = avatarService;
      this._gameMessageService = gameMessageService;
      this.FlowerImage = (Image) null;
      this.FeatherImage = (Image) null;
      this.SandImage = (Image) null;
      this.GobletImage = (Image) null;
      this.HeadImage = (Image) null;
      this.FlowerLabel = string.Empty;
      this.FeatherLabel = string.Empty;
      this.SandLabel = string.Empty;
      this.GobletLabel = string.Empty;
      this.HeadLabel = string.Empty;
    }

    private async void btnQuickEquip_Click(object sender, EventArgs e)
    {
      string avatarName;
      if (this.RelicProfile == null)
      {
        avatarName = (string) null;
      }
      else
      {
        avatarName = this.RelicProfile.UserAvatar.Avatar.Name;
        await this.EquipToAvatar(this.RelicProfile.AvatarGuid);
        GameMessageService gameMessageService = this._gameMessageService;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
        interpolatedStringHandler.AppendLiteral("已为 [");
        interpolatedStringHandler.AppendFormatted(avatarName);
        interpolatedStringHandler.AppendLiteral("] 装备 [");
        interpolatedStringHandler.AppendFormatted(this.RelicProfile.ProfileName);
        interpolatedStringHandler.AppendLiteral("] 预设");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        gameMessageService.EnqueueMessage(stringAndClear);
        avatarName = (string) null;
      }
    }

    private async void btnEquipTo_Click(object sender, EventArgs e)
    {
      string profileAvatarName;
      string avatarName;
      if (this.RelicProfile == null)
      {
        profileAvatarName = (string) null;
        avatarName = (string) null;
      }
      else
      {
        AvatarSelectionForm requiredService = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
        if (requiredService.ShowDialog() == DialogResult.Cancel)
        {
          profileAvatarName = (string) null;
          avatarName = (string) null;
        }
        else
        {
          ulong selectedAvatarGuid = requiredService.SelectedAvatarGuid;
          DbUserAvatar userAvatarByGuid = await this._avatarService.GetUserAvatarByGuid(selectedAvatarGuid);
          if (userAvatarByGuid == null)
          {
            profileAvatarName = (string) null;
            avatarName = (string) null;
          }
          else
          {
            profileAvatarName = this.RelicProfile.UserAvatar.Avatar.Name;
            avatarName = userAvatarByGuid.Avatar.Name;
            await this.EquipToAvatar(selectedAvatarGuid);
            GameMessageService gameMessageService = this._gameMessageService;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 3);
            interpolatedStringHandler.AppendLiteral("已为 [");
            interpolatedStringHandler.AppendFormatted(avatarName);
            interpolatedStringHandler.AppendLiteral("] 装备 [");
            interpolatedStringHandler.AppendFormatted(profileAvatarName);
            interpolatedStringHandler.AppendLiteral("->");
            interpolatedStringHandler.AppendFormatted(this.RelicProfile.ProfileName);
            interpolatedStringHandler.AppendLiteral("] 预设");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            gameMessageService.EnqueueMessage(stringAndClear);
            profileAvatarName = (string) null;
            avatarName = (string) null;
          }
        }
      }
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
      ProfileEditForm requiredService = Program.ServiceProvider.GetRequiredService<ProfileEditForm>();
      requiredService.RelicProfile = this.RelicProfile;
      int num = (int) requiredService.ShowDialog();
    }

    private void PictureBox_Resize(object sender, EventArgs e)
    {
      bool flag = false;
      try
      {
        Monitor.TryEnter(this._lock, ref flag);
        if (!flag)
          return;
        PictureBox pictureBox1 = sender as PictureBox;
        PictureBox pictureBox2 = pictureBox1;
        Size size1 = pictureBox1.Size;
        size1.Height = pictureBox1.Size.Width;
        Size size2 = size1;
        pictureBox2.Size = size2;
      }
      catch (Exception ex)
      {
      }
      finally
      {
        if (flag)
          Monitor.Exit(this._lock);
      }
    }

    private async Task EquipToAvatar(ulong avatarGuid)
    {
      if (this.RelicProfile?.RelicItems == null)
        return;
      foreach (DbRelicItem relicItem in (IEnumerable<DbRelicItem>) this.RelicProfile.RelicItems)
      {
        ulong guid = relicItem.Guid;
        int num = await this._equipService.WearEquip(avatarGuid, guid) ? 1 : 0;
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
      this.components = (IContainer) new Container();
      this.groupBox = new GroupBox();
      this.tableLayoutPanel = new TableLayoutPanel();
      this.labelHead = new Label();
      this.labelGoblet = new Label();
      this.labelSand = new Label();
      this.labelFeather = new Label();
      this.pbHead = new PictureBox();
      this.pbGoblet = new PictureBox();
      this.pbSand = new PictureBox();
      this.pbFeather = new PictureBox();
      this.pbFlower = new PictureBox();
      this.labelFlower = new Label();
      this.btnEdit = new Button();
      this.btnQuickEquip = new Button();
      this.btnEquipTo = new Button();
      this.labelConflict = new Label();
      this.toolTip = new ToolTip(this.components);
      this.groupBox.SuspendLayout();
      this.tableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pbHead).BeginInit();
      ((ISupportInitialize) this.pbGoblet).BeginInit();
      ((ISupportInitialize) this.pbSand).BeginInit();
      ((ISupportInitialize) this.pbFeather).BeginInit();
      ((ISupportInitialize) this.pbFlower).BeginInit();
      this.SuspendLayout();
      this.groupBox.Controls.Add((Control) this.tableLayoutPanel);
      this.groupBox.Dock = DockStyle.Fill;
      this.groupBox.Location = new Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Padding = new Padding(5, 3, 5, 5);
      this.groupBox.Size = new Size(875, 170);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "<预设名>";
      this.tableLayoutPanel.ColumnCount = 10;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
      this.tableLayoutPanel.Controls.Add((Control) this.labelHead, 9, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelGoblet, 7, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelSand, 5, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelFeather, 3, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbHead, 8, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbGoblet, 6, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbSand, 4, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbFeather, 2, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbFlower, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelFlower, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.btnEdit, 9, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.btnQuickEquip, 8, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.btnEquipTo, 7, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.labelConflict, 0, 2);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(5, 19);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 3;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel.Size = new Size(865, 146);
      this.tableLayoutPanel.TabIndex = 0;
      this.labelHead.Dock = DockStyle.Fill;
      this.labelHead.Location = new Point(777, 3);
      this.labelHead.Margin = new Padding(3);
      this.labelHead.Name = "labelHead";
      this.tableLayoutPanel.SetRowSpan((Control) this.labelHead, 2);
      this.labelHead.Size = new Size(85, 110);
      this.labelHead.TabIndex = 9;
      this.labelHead.Text = "label1";
      this.labelGoblet.Dock = DockStyle.Fill;
      this.labelGoblet.Location = new Point(605, 3);
      this.labelGoblet.Margin = new Padding(3);
      this.labelGoblet.Name = "labelGoblet";
      this.tableLayoutPanel.SetRowSpan((Control) this.labelGoblet, 2);
      this.labelGoblet.Size = new Size(80, 110);
      this.labelGoblet.TabIndex = 8;
      this.labelGoblet.Text = "label1";
      this.labelSand.Dock = DockStyle.Fill;
      this.labelSand.Location = new Point(433, 3);
      this.labelSand.Margin = new Padding(3);
      this.labelSand.Name = "labelSand";
      this.tableLayoutPanel.SetRowSpan((Control) this.labelSand, 2);
      this.labelSand.Size = new Size(80, 110);
      this.labelSand.TabIndex = 7;
      this.labelSand.Text = "label1";
      this.labelFeather.Dock = DockStyle.Fill;
      this.labelFeather.Location = new Point(261, 3);
      this.labelFeather.Margin = new Padding(3);
      this.labelFeather.Name = "labelFeather";
      this.tableLayoutPanel.SetRowSpan((Control) this.labelFeather, 2);
      this.labelFeather.Size = new Size(80, 110);
      this.labelFeather.TabIndex = 6;
      this.labelFeather.Text = "label1";
      this.pbHead.Image = (Image) Resources.dot_green;
      this.pbHead.Location = new Point(691, 3);
      this.pbHead.MaximumSize = new Size(80, 80);
      this.pbHead.MinimumSize = new Size(60, 60);
      this.pbHead.Name = "pbHead";
      this.pbHead.Size = new Size(80, 80);
      this.pbHead.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbHead.TabIndex = 5;
      this.pbHead.TabStop = false;
      this.pbHead.Resize += new EventHandler(this.PictureBox_Resize);
      this.pbGoblet.Image = (Image) Resources.dot_green;
      this.pbGoblet.Location = new Point(519, 3);
      this.pbGoblet.MaximumSize = new Size(80, 80);
      this.pbGoblet.MinimumSize = new Size(60, 60);
      this.pbGoblet.Name = "pbGoblet";
      this.pbGoblet.Size = new Size(80, 80);
      this.pbGoblet.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbGoblet.TabIndex = 4;
      this.pbGoblet.TabStop = false;
      this.pbGoblet.Resize += new EventHandler(this.PictureBox_Resize);
      this.pbSand.Image = (Image) Resources.dot_green;
      this.pbSand.Location = new Point(347, 3);
      this.pbSand.MaximumSize = new Size(80, 80);
      this.pbSand.MinimumSize = new Size(60, 60);
      this.pbSand.Name = "pbSand";
      this.pbSand.Size = new Size(80, 80);
      this.pbSand.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbSand.TabIndex = 3;
      this.pbSand.TabStop = false;
      this.pbSand.Resize += new EventHandler(this.PictureBox_Resize);
      this.pbFeather.Image = (Image) Resources.dot_green;
      this.pbFeather.Location = new Point(175, 3);
      this.pbFeather.MaximumSize = new Size(80, 80);
      this.pbFeather.MinimumSize = new Size(60, 60);
      this.pbFeather.Name = "pbFeather";
      this.pbFeather.Size = new Size(80, 80);
      this.pbFeather.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbFeather.TabIndex = 2;
      this.pbFeather.TabStop = false;
      this.pbFeather.Resize += new EventHandler(this.PictureBox_Resize);
      this.pbFlower.Image = (Image) Resources.dot_green;
      this.pbFlower.Location = new Point(3, 3);
      this.pbFlower.MaximumSize = new Size(80, 80);
      this.pbFlower.MinimumSize = new Size(60, 60);
      this.pbFlower.Name = "pbFlower";
      this.pbFlower.Size = new Size(80, 80);
      this.pbFlower.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbFlower.TabIndex = 0;
      this.pbFlower.TabStop = false;
      this.pbFlower.Resize += new EventHandler(this.PictureBox_Resize);
      this.labelFlower.Dock = DockStyle.Fill;
      this.labelFlower.Font = new Font("Segoe UI", 9f, (FontStyle) 0, (GraphicsUnit) 3, (byte) 0);
      this.labelFlower.Location = new Point(89, 3);
      this.labelFlower.Margin = new Padding(3);
      this.labelFlower.Name = "labelFlower";
      this.tableLayoutPanel.SetRowSpan((Control) this.labelFlower, 2);
      this.labelFlower.Size = new Size(80, 110);
      this.labelFlower.TabIndex = 1;
      this.labelFlower.Text = "label1";
      this.btnEdit.Dock = DockStyle.Fill;
      this.btnEdit.Location = new Point(777, 119);
      this.btnEdit.Name = "btnEdit";
      this.btnEdit.Size = new Size(85, 24);
      this.btnEdit.TabIndex = 10;
      this.btnEdit.Text = "编辑";
      this.toolTip.SetToolTip((Control) this.btnEdit, "更改设置或删除");
      this.btnEdit.UseVisualStyleBackColor = true;
      this.btnEdit.Click += new EventHandler(this.btnEdit_Click);
      this.btnQuickEquip.Dock = DockStyle.Fill;
      this.btnQuickEquip.Location = new Point(691, 119);
      this.btnQuickEquip.Name = "btnQuickEquip";
      this.btnQuickEquip.Size = new Size(80, 24);
      this.btnQuickEquip.TabIndex = 11;
      this.btnQuickEquip.Text = "快速装备";
      this.toolTip.SetToolTip((Control) this.btnQuickEquip, "装备到当前角色");
      this.btnQuickEquip.UseVisualStyleBackColor = true;
      this.btnQuickEquip.Click += new EventHandler(this.btnQuickEquip_Click);
      this.btnEquipTo.Dock = DockStyle.Fill;
      this.btnEquipTo.Location = new Point(605, 119);
      this.btnEquipTo.Name = "btnEquipTo";
      this.btnEquipTo.Size = new Size(80, 24);
      this.btnEquipTo.TabIndex = 12;
      this.btnEquipTo.Text = "装备到";
      this.toolTip.SetToolTip((Control) this.btnEquipTo, "装备到其他角色");
      this.btnEquipTo.UseVisualStyleBackColor = true;
      this.btnEquipTo.Click += new EventHandler(this.btnEquipTo_Click);
      this.labelConflict.AutoSize = true;
      this.tableLayoutPanel.SetColumnSpan((Control) this.labelConflict, 6);
      this.labelConflict.Font = new Font("Segoe UI", 9f, (FontStyle) 1, (GraphicsUnit) 3, (byte) 0);
      this.labelConflict.ForeColor = Color.Red;
      this.labelConflict.Location = new Point(3, 123);
      this.labelConflict.Margin = new Padding(3, 7, 5, 5);
      this.labelConflict.Name = "labelConflict";
      this.labelConflict.Size = new Size(35, 15);
      this.labelConflict.TabIndex = 13;
      this.labelConflict.Text = "冲突";
      this.toolTip.AutomaticDelay = 0;
      this.toolTip.AutoPopDelay = 5000;
      this.toolTip.InitialDelay = 0;
      this.toolTip.ReshowDelay = 0;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.groupBox);
      this.Name = nameof (ProfileDetails);
      this.Size = new Size(875, 170);
      this.groupBox.ResumeLayout(false);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      ((ISupportInitialize) this.pbHead).EndInit();
      ((ISupportInitialize) this.pbGoblet).EndInit();
      ((ISupportInitialize) this.pbSand).EndInit();
      ((ISupportInitialize) this.pbFeather).EndInit();
      ((ISupportInitialize) this.pbFlower).EndInit();
      this.ResumeLayout(false);
    }
  }
}
