// Decompiled with JetBrains decompiler
// Type: RelicService.View.AvatarTeamItem
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  public class AvatarTeamItem : UserControl
  {
    private const float WidthToHeightScale = 0.308695644f;
    private 
    #nullable disable
    IContainer components;
    private TableLayoutPanel tableLayoutPanel;
    private PictureBox pbAvatar1;
    private PictureBox pbAvatar3;
    private PictureBox pbAvatar2;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem menuDelete;

    public int ControlIndex { get; set; }

    public 
    #nullable enable
    Action<AvatarTeamItem, int>? OnDeleteCallback { set; get; }

    public Image AvatarImage1
    {
      set => this.pbAvatar1.Image = value;
    }

    public Image AvatarImage2
    {
      set => this.pbAvatar2.Image = value;
    }

    public Image AvatarImage3
    {
      set => this.pbAvatar3.Image = value;
    }

    public AvatarTeamItem()
    {
      this.InitializeComponent();
      Size size1 = this.Size;
      Size size2 = size1;
      size2.Height = (int) ((double) size1.Width * 0.30869564414024353);
      this.Size = size2;
    }

    private void menuDelete_Click(object sender, EventArgs e)
    {
      Action<AvatarTeamItem, int> onDeleteCallback = this.OnDeleteCallback;
      if (onDeleteCallback == null)
        return;
      onDeleteCallback(this, this.ControlIndex);
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
      this.contextMenu = new ContextMenuStrip(this.components);
      this.menuDelete = new ToolStripMenuItem();
      this.pbAvatar3 = new PictureBox();
      this.pbAvatar2 = new PictureBox();
      this.pbAvatar1 = new PictureBox();
      this.tableLayoutPanel.SuspendLayout();
      this.contextMenu.SuspendLayout();
      ((ISupportInitialize) this.pbAvatar3).BeginInit();
      ((ISupportInitialize) this.pbAvatar2).BeginInit();
      ((ISupportInitialize) this.pbAvatar1).BeginInit();
      this.SuspendLayout();
      this.tableLayoutPanel.ColumnCount = 3;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321f));
      this.tableLayoutPanel.ContextMenuStrip = this.contextMenu;
      this.tableLayoutPanel.Controls.Add((Control) this.pbAvatar3, 2, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbAvatar2, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.pbAvatar1, 0, 0);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.Size = new Size(230, 71);
      this.tableLayoutPanel.TabIndex = 0;
      this.contextMenu.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.menuDelete
      });
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new Size(181, 48);
      this.menuDelete.Name = "menuDelete";
      this.menuDelete.Size = new Size(180, 22);
      this.menuDelete.Text = "删除";
      this.menuDelete.Click += new EventHandler(this.menuDelete_Click);
      this.pbAvatar3.ContextMenuStrip = this.contextMenu;
      this.pbAvatar3.Location = new Point(158, 3);
      this.pbAvatar3.Margin = new Padding(6, 3, 3, 3);
      this.pbAvatar3.Name = "pbAvatar3";
      this.pbAvatar3.Size = new Size(65, 65);
      this.pbAvatar3.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbAvatar3.TabIndex = 2;
      this.pbAvatar3.TabStop = false;
      this.pbAvatar2.ContextMenuStrip = this.contextMenu;
      this.pbAvatar2.Location = new Point(82, 3);
      this.pbAvatar2.Margin = new Padding(6, 3, 3, 3);
      this.pbAvatar2.Name = "pbAvatar2";
      this.pbAvatar2.Size = new Size(65, 65);
      this.pbAvatar2.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbAvatar2.TabIndex = 1;
      this.pbAvatar2.TabStop = false;
      this.pbAvatar1.ContextMenuStrip = this.contextMenu;
      this.pbAvatar1.Location = new Point(6, 3);
      this.pbAvatar1.Margin = new Padding(6, 3, 3, 3);
      this.pbAvatar1.Name = "pbAvatar1";
      this.pbAvatar1.Size = new Size(65, 65);
      this.pbAvatar1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbAvatar1.TabIndex = 0;
      this.pbAvatar1.TabStop = false;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.tableLayoutPanel);
      this.Name = nameof (AvatarTeamItem);
      this.Size = new Size(230, 71);
      this.tableLayoutPanel.ResumeLayout(false);
      this.contextMenu.ResumeLayout(false);
      ((ISupportInitialize) this.pbAvatar3).EndInit();
      ((ISupportInitialize) this.pbAvatar2).EndInit();
      ((ISupportInitialize) this.pbAvatar1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
