// Decompiled with JetBrains decompiler
// Type: RelicService.View.AvatarRelicInfo
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Properties;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  public class AvatarRelicInfo : UserControl
  {
    private bool _isSelected;
    private ulong _avatarGuid;
    private Action<ulong>? _onAnyItemClicked;
    private 
    #nullable disable
    IContainer components;
    private TableLayoutPanel tableLayoutPanel;
    private PictureBox pbAvatar;
    private PictureBox pbFlower;
    private PictureBox pbFeather;
    private PictureBox pbSand;
    private PictureBox pbGoblet;
    private PictureBox pbHead;
    private ToolTip toolTip;

    public bool IsSelected
    {
      get => this._isSelected;
      set
      {
        this._isSelected = value;
        this.tableLayoutPanel.BackColor = this._isSelected ? SystemColors.ControlDark : SystemColors.Menu;
      }
    }

    public 
    #nullable enable
    Image AvatarImage
    {
      set => this.pbAvatar.Image = value;
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

    public string FlowerTooltip
    {
      set => this.toolTip.SetToolTip((Control) this.pbFlower, value);
    }

    public string FeatherTooltip
    {
      set => this.toolTip.SetToolTip((Control) this.pbFeather, value);
    }

    public string SandTooltip
    {
      set => this.toolTip.SetToolTip((Control) this.pbSand, value);
    }

    public string GobletTooltip
    {
      set => this.toolTip.SetToolTip((Control) this.pbGoblet, value);
    }

    public string HeadTooltip
    {
      set => this.toolTip.SetToolTip((Control) this.pbHead, value);
    }

    public Action<ulong> OnClickCallback
    {
      set => this._onAnyItemClicked = value;
    }

    public AvatarRelicInfo(ulong avatarGuid)
    {
      this.InitializeComponent();
      this._avatarGuid = avatarGuid;
      Enumerable.ToList<PictureBox>(Enumerable.OfType<PictureBox>((IEnumerable) this.tableLayoutPanel.Controls)).ForEach((Action<PictureBox>) (pb =>
      {
        pb.Image = (Image) null;
        pb.Click += (EventHandler) ((sender, e) =>
        {
          Action<ulong> onAnyItemClicked = this._onAnyItemClicked;
          if (onAnyItemClicked == null)
            return;
          onAnyItemClicked(this._avatarGuid);
        });
      }));
      this.tableLayoutPanel.Click += (EventHandler) ((sender, e) =>
      {
        Action<ulong> onAnyItemClicked = this._onAnyItemClicked;
        if (onAnyItemClicked == null)
          return;
        onAnyItemClicked(this._avatarGuid);
      });
      this.Click += (EventHandler) ((sender, e) =>
      {
        Action<ulong> onAnyItemClicked = this._onAnyItemClicked;
        if (onAnyItemClicked == null)
          return;
        onAnyItemClicked(this._avatarGuid);
      });
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
      this.tableLayoutPanel = new TableLayoutPanel();
      this.pbAvatar = new PictureBox();
      this.pbFlower = new PictureBox();
      this.pbFeather = new PictureBox();
      this.pbSand = new PictureBox();
      this.pbGoblet = new PictureBox();
      this.pbHead = new PictureBox();
      this.toolTip = new ToolTip(this.components);
      this.tableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pbAvatar).BeginInit();
      ((ISupportInitialize) this.pbFlower).BeginInit();
      ((ISupportInitialize) this.pbFeather).BeginInit();
      ((ISupportInitialize) this.pbSand).BeginInit();
      ((ISupportInitialize) this.pbGoblet).BeginInit();
      ((ISupportInitialize) this.pbHead).BeginInit();
      this.SuspendLayout();
      this.tableLayoutPanel.BackColor = SystemColors.Menu;
      this.tableLayoutPanel.ColumnCount = 6;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel.Controls.Add((Control) this.pbAvatar, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbFlower, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbFeather, 2, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbSand, 3, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbGoblet, 4, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbHead, 5, 0);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.Size = new Size(900, 150);
      this.tableLayoutPanel.TabIndex = 0;
      this.pbAvatar.Dock = DockStyle.Fill;
      this.pbAvatar.Image = (Image) Resources.dot_green;
      this.pbAvatar.Location = new Point(5, 5);
      this.pbAvatar.Margin = new Padding(5);
      this.pbAvatar.Name = "pbAvatar";
      this.pbAvatar.Size = new Size(140, 140);
      this.pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbAvatar.TabIndex = 0;
      this.pbAvatar.TabStop = false;
      this.pbFlower.Dock = DockStyle.Fill;
      this.pbFlower.Image = (Image) Resources.dot_red;
      this.pbFlower.Location = new Point(155, 5);
      this.pbFlower.Margin = new Padding(5);
      this.pbFlower.Name = "pbFlower";
      this.pbFlower.Size = new Size(140, 140);
      this.pbFlower.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbFlower.TabIndex = 1;
      this.pbFlower.TabStop = false;
      this.pbFeather.Dock = DockStyle.Fill;
      this.pbFeather.Image = (Image) Resources.dot_yellow;
      this.pbFeather.Location = new Point(305, 5);
      this.pbFeather.Margin = new Padding(5);
      this.pbFeather.Name = "pbFeather";
      this.pbFeather.Size = new Size(140, 140);
      this.pbFeather.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbFeather.TabIndex = 2;
      this.pbFeather.TabStop = false;
      this.pbSand.Dock = DockStyle.Fill;
      this.pbSand.Image = (Image) Resources.dot_green;
      this.pbSand.Location = new Point(455, 5);
      this.pbSand.Margin = new Padding(5);
      this.pbSand.Name = "pbSand";
      this.pbSand.Size = new Size(140, 140);
      this.pbSand.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbSand.TabIndex = 3;
      this.pbSand.TabStop = false;
      this.pbGoblet.Dock = DockStyle.Fill;
      this.pbGoblet.Image = (Image) Resources.dot_red;
      this.pbGoblet.Location = new Point(605, 5);
      this.pbGoblet.Margin = new Padding(5);
      this.pbGoblet.Name = "pbGoblet";
      this.pbGoblet.Size = new Size(140, 140);
      this.pbGoblet.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbGoblet.TabIndex = 4;
      this.pbGoblet.TabStop = false;
      this.pbHead.Dock = DockStyle.Fill;
      this.pbHead.Image = (Image) Resources.dot_yellow;
      this.pbHead.Location = new Point(755, 5);
      this.pbHead.Margin = new Padding(5);
      this.pbHead.Name = "pbHead";
      this.pbHead.Size = new Size(140, 140);
      this.pbHead.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbHead.TabIndex = 5;
      this.pbHead.TabStop = false;
      this.toolTip.AutomaticDelay = 0;
      this.toolTip.AutoPopDelay = 15000;
      this.toolTip.InitialDelay = 0;
      this.toolTip.ReshowDelay = 0;
      this.toolTip.UseAnimation = false;
      this.toolTip.UseFading = false;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Control;
      this.Controls.Add((Control) this.tableLayoutPanel);
      this.Name = nameof (AvatarRelicInfo);
      this.Size = new Size(900, 150);
      this.tableLayoutPanel.ResumeLayout(false);
      ((ISupportInitialize) this.pbAvatar).EndInit();
      ((ISupportInitialize) this.pbFlower).EndInit();
      ((ISupportInitialize) this.pbFeather).EndInit();
      ((ISupportInitialize) this.pbSand).EndInit();
      ((ISupportInitialize) this.pbGoblet).EndInit();
      ((ISupportInitialize) this.pbHead).EndInit();
      this.ResumeLayout(false);
    }
  }
}
