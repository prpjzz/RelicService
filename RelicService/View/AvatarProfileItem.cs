// Decompiled with JetBrains decompiler
// Type: RelicService.View.AvatarProfileItem
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Properties;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  public class AvatarProfileItem : UserControl
  {
    private static readonly float WidthToHeightScale = 0.2777778f;
    private bool _isSelected;
    private ulong _avatarGuid;
    private Action<ulong>? _onAnyItemClicked;
    private readonly object _lock = new object();
    private 
    #nullable disable
    IContainer components;
    private TableLayoutPanel tableLayoutPanel;
    private PictureBox pbAvatar;
    private Label labelAvatarName;

    public bool IsSelected
    {
      get => this._isSelected;
      set
      {
        this._isSelected = value;
        this.tableLayoutPanel.BackColor = this._isSelected ? SystemColors.ControlDark : SystemColors.Window;
      }
    }

    public 
    #nullable enable
    Image AvatarImage
    {
      set => this.pbAvatar.Image = value;
    }

    public string AvatarName
    {
      get => this.labelAvatarName.Text;
      set => this.labelAvatarName.Text = value;
    }

    public Action<ulong> OnClickCallback
    {
      set => this._onAnyItemClicked = value;
    }

    public AvatarProfileItem()
    {
    }

    public AvatarProfileItem(ulong avatarGuid)
    {
      this.InitializeComponent();
      this._avatarGuid = avatarGuid;
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) this.tableLayoutPanel.Controls)).ForEach((Action<Control>) (c => c.Click += (EventHandler) ((sender, e) =>
      {
        Action<ulong> onAnyItemClicked = this._onAnyItemClicked;
        if (onAnyItemClicked == null)
          return;
        onAnyItemClicked(this._avatarGuid);
      })));
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
      this.pbAvatar.Image = (Image) null;
      Size size1 = this.Size;
      Size size2 = size1;
      size2.Height = (int) ((double) size1.Width * (double) AvatarProfileItem.WidthToHeightScale);
      this.Size = size2;
    }

    private void pbAvatar_Resize(object sender, EventArgs e)
    {
      bool flag = false;
      try
      {
        Monitor.TryEnter(this._lock, ref flag);
        if (!flag)
          return;
        PictureBox pictureBox1 = (PictureBox) sender;
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

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        ((IDisposable) this.components).Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tableLayoutPanel = new TableLayoutPanel();
      this.pbAvatar = new PictureBox();
      this.labelAvatarName = new Label();
      this.tableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pbAvatar).BeginInit();
      this.SuspendLayout();
      this.tableLayoutPanel.ColumnCount = 2;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72f));
      this.tableLayoutPanel.Controls.Add((Control) this.pbAvatar, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelAvatarName, 1, 0);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.Size = new Size(270, 75);
      this.tableLayoutPanel.TabIndex = 0;
      this.pbAvatar.Dock = DockStyle.Fill;
      this.pbAvatar.Image = (Image) Resources.dot_green;
      this.pbAvatar.Location = new Point(5, 5);
      this.pbAvatar.Margin = new Padding(5);
      this.pbAvatar.Name = "pbAvatar";
      this.pbAvatar.Size = new Size(65, 65);
      this.pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pbAvatar.TabIndex = 0;
      this.pbAvatar.TabStop = false;
      this.pbAvatar.Resize += new EventHandler(this.pbAvatar_Resize);
      this.labelAvatarName.Dock = DockStyle.Fill;
      this.labelAvatarName.Font = new Font("Segoe UI", 14.25f, (FontStyle) 0, (GraphicsUnit) 3, (byte) 0);
      this.labelAvatarName.Location = new Point(78, 0);
      this.labelAvatarName.Name = "labelAvatarName";
      this.labelAvatarName.Padding = new Padding(40, 0, 0, 0);
      this.labelAvatarName.Size = new Size(189, 75);
      this.labelAvatarName.TabIndex = 1;
      this.labelAvatarName.Text = "label1";
      this.labelAvatarName.TextAlign = (ContentAlignment) 16;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.tableLayoutPanel);
      this.Name = nameof (AvatarProfileItem);
      this.Size = new Size(270, 75);
      this.tableLayoutPanel.ResumeLayout(false);
      ((ISupportInitialize) this.pbAvatar).EndInit();
      this.ResumeLayout(false);
    }
  }
}
