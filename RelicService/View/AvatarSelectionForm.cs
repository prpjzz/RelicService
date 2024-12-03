// Decompiled with JetBrains decompiler
// Type: RelicService.View.AvatarSelectionForm
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Database;
using RelicService.Service;
using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  internal class AvatarSelectionForm : Form
  {
    private AvatarService _avatarService;
    private StatusService _statusService;
    private ResourceManager _resourceManager;
    private HashSet<uint> _avatarImageRef = new HashSet<uint>();
    private PictureBox? _lastSelectedPictureBox;
    private 
    #nullable disable
    IContainer components;
    private TableLayoutPanel tableLayoutPanel1;
    private Button btnConfirm;
    private GroupBox groupBox;
    private TableLayoutPanel tlpAvatars;

    public bool IsMultiSelect { get; set; }

    public uint MultiSelectLimit { get; set; } = 1;

    public 
    #nullable enable
    List<ulong> SelectedAvatarGuids { get; private set; } = new List<ulong>();

    public ulong SelectedAvatarGuid
    {
      get => this._lastSelectedPictureBox == null ? 0UL : (ulong) this._lastSelectedPictureBox.Tag;
    }

    public AvatarSelectionForm()
    {
    }

    public AvatarSelectionForm(
      AvatarService avatarService,
      StatusService statusService,
      ResourceManager resourceManager)
    {
      this.InitializeComponent();
      this._avatarService = avatarService;
      this._statusService = statusService;
      this._resourceManager = resourceManager;
    }

    private async void AvatarSelectionForm_Load(object sender, EventArgs e)
    {
      this.btnConfirm.Enabled = false;
      await this.PopulateGrid();
    }

    private void AvatarSelectionForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Enumerable.ToList<uint>((IEnumerable<uint>) this._avatarImageRef).ForEach(new Action<uint>(this._resourceManager.FreeAvatarImage));
    }

    private void AvatarPictureBox_Click(object? sender, EventArgs e)
    {
      if (!(sender is PictureBox pictureBox))
        return;
      if (!this.IsMultiSelect)
      {
        if (this._lastSelectedPictureBox != null)
          this._lastSelectedPictureBox.BackColor = Color.Transparent;
        pictureBox.BackColor = SystemColors.ControlDark;
        this._lastSelectedPictureBox = pictureBox;
      }
      else
      {
        if (Color.op_Equality(pictureBox.BackColor, SystemColors.ControlDark))
        {
          pictureBox.BackColor = Color.Transparent;
          this.SelectedAvatarGuids.Remove((ulong) pictureBox.Tag);
        }
        else
        {
          if ((long) this.SelectedAvatarGuids.Count >= (long) this.MultiSelectLimit)
            return;
          pictureBox.BackColor = SystemColors.ControlDark;
          this.SelectedAvatarGuids.Add((ulong) pictureBox.Tag);
        }
        this.btnConfirm.Enabled = this.SelectedAvatarGuids.Count > 0;
      }
      this.btnConfirm.Enabled = true;
    }

    private void AvatarPictureBox_DoubleClick(object? sender, EventArgs e)
    {
      if (this._lastSelectedPictureBox == null)
        return;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnConfirm_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private async Task PopulateGrid()
    {
      AvatarSelectionForm avatarSelectionForm = this;
      List<DbUserAvatar> userAvatars = await avatarSelectionForm._avatarService.GetUserAvatars(avatarSelectionForm._statusService.CurrentUid);
      if (userAvatars == null)
        return;
      foreach (DbUserAvatar dbUserAvatar in Enumerable.ToList<DbUserAvatar>((IEnumerable<DbUserAvatar>) Enumerable.OrderBy<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) userAvatars, (Func<DbUserAvatar, uint>) (x => x.AvatarId))))
      {
        DbUserAvatar userAvatar = dbUserAvatar;
        Image avatarImage = await avatarSelectionForm._resourceManager.GetAvatarImage(userAvatar.AvatarId);
        if (avatarImage != null)
        {
          Image image = Utils.ResizeImage(avatarImage, 80, 80);
          avatarSelectionForm._avatarImageRef.Add(userAvatar.AvatarId);
          PictureBox pictureBox1 = new PictureBox();
          pictureBox1.Image = image;
          pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
          pictureBox1.Tag = (object) userAvatar.Guid;
          pictureBox1.Size = new Size(80, 80);
          pictureBox1.Margin = new Padding(8, 4, 3, 3);
          pictureBox1.Cursor = Cursors.Hand;
          PictureBox pictureBox2 = pictureBox1;
          pictureBox2.Click += new EventHandler(avatarSelectionForm.AvatarPictureBox_Click);
          pictureBox2.DoubleClick += new EventHandler(avatarSelectionForm.AvatarPictureBox_DoubleClick);
          avatarSelectionForm.tlpAvatars.Controls.Add((Control) pictureBox2);
          userAvatar = (DbUserAvatar) null;
        }
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
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.btnConfirm = new Button();
      this.groupBox = new GroupBox();
      this.tlpAvatars = new TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel1.Controls.Add((Control) this.btnConfirm, 0, 1);
      this.tableLayoutPanel1.Controls.Add((Control) this.groupBox, 0, 0);
      this.tableLayoutPanel1.Dock = DockStyle.Fill;
      this.tableLayoutPanel1.Location = new Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 96f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4f));
      this.tableLayoutPanel1.Size = new Size(784, 761);
      this.tableLayoutPanel1.TabIndex = 0;
      this.btnConfirm.Dock = DockStyle.Right;
      this.btnConfirm.Location = new Point(706, 733);
      this.btnConfirm.Name = "btnConfirm";
      this.btnConfirm.Size = new Size(75, 25);
      this.btnConfirm.TabIndex = 0;
      this.btnConfirm.Text = "确定";
      this.btnConfirm.UseVisualStyleBackColor = true;
      this.btnConfirm.Click += new EventHandler(this.btnConfirm_Click);
      this.groupBox.Controls.Add((Control) this.tlpAvatars);
      this.groupBox.Dock = DockStyle.Fill;
      this.groupBox.Location = new Point(3, 3);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new Size(778, 724);
      this.groupBox.TabIndex = 1;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "角色";
      this.tlpAvatars.AutoScroll = true;
      this.tlpAvatars.BackColor = SystemColors.Window;
      this.tlpAvatars.ColumnCount = 8;
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
      this.tlpAvatars.Location = new Point(3, 19);
      this.tlpAvatars.Name = "tlpAvatars";
      this.tlpAvatars.RowCount = 12;
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.RowStyles.Add(new RowStyle());
      this.tlpAvatars.Size = new Size(772, 699);
      this.tlpAvatars.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(784, 761);
      this.Controls.Add((Control) this.tableLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AvatarSelectionForm);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "角色选择";
      this.FormClosed += new FormClosedEventHandler(this.AvatarSelectionForm_FormClosed);
      this.Load += new EventHandler(this.AvatarSelectionForm_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
