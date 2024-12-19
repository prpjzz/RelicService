using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RelicService.Data.Database;
using RelicService.Service;
using RelicService.Tools;

namespace RelicService.View;

internal class AvatarSelectionForm : Form
{
	private AvatarService _avatarService;

	private StatusService _statusService;

	private ResourceManager _resourceManager;

	private HashSet<uint> _avatarImageRef = new HashSet<uint>();

	private PictureBox? _lastSelectedPictureBox;

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel1;

	private Button btnConfirm;

	private GroupBox groupBox;

	private TableLayoutPanel tlpAvatars;

	public bool IsMultiSelect { get; set; }

	public uint MultiSelectLimit { get; set; } = 1u;

	public List<ulong> SelectedAvatarGuids { get; private set; } = new List<ulong>();

	public ulong SelectedAvatarGuid
	{
		get
		{
			if (_lastSelectedPictureBox == null)
			{
				return 0uL;
			}
			return (ulong)_lastSelectedPictureBox.Tag;
		}
	}

	public AvatarSelectionForm()
	{
	}

	public AvatarSelectionForm(AvatarService avatarService, StatusService statusService, ResourceManager resourceManager)
	{
		InitializeComponent();
		_avatarService = avatarService;
		_statusService = statusService;
		_resourceManager = resourceManager;
	}

	private async void AvatarSelectionForm_Load(object sender, EventArgs e)
	{
		btnConfirm.Enabled = false;
		await PopulateGrid();
	}

	private void AvatarSelectionForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		_avatarImageRef.ToList().ForEach(_resourceManager.FreeAvatarImage);
	}

	private void AvatarPictureBox_Click(object? sender, EventArgs e)
	{
		if (!(sender is PictureBox pictureBox))
		{
			return;
		}
		if (!IsMultiSelect)
		{
			if (_lastSelectedPictureBox != null)
			{
				_lastSelectedPictureBox.BackColor = Color.Transparent;
			}
			pictureBox.BackColor = SystemColors.ControlDark;
			_lastSelectedPictureBox = pictureBox;
		}
		else
		{
			if (pictureBox.BackColor == SystemColors.ControlDark)
			{
				pictureBox.BackColor = Color.Transparent;
				SelectedAvatarGuids.Remove((ulong)pictureBox.Tag);
			}
			else
			{
				if (SelectedAvatarGuids.Count >= MultiSelectLimit)
				{
					return;
				}
				pictureBox.BackColor = SystemColors.ControlDark;
				SelectedAvatarGuids.Add((ulong)pictureBox.Tag);
			}
			btnConfirm.Enabled = SelectedAvatarGuids.Count > 0;
		}
		btnConfirm.Enabled = true;
	}

	private void AvatarPictureBox_DoubleClick(object? sender, EventArgs e)
	{
		if (_lastSelectedPictureBox != null)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void btnConfirm_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private async Task PopulateGrid()
	{
		List<DbUserAvatar> list = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
		if (list == null)
		{
			return;
		}
		list = list.OrderBy((DbUserAvatar x) => x.AvatarId).ToList();
		foreach (DbUserAvatar userAvatar in list)
		{
			Image image = await _resourceManager.GetAvatarImage(userAvatar.AvatarId);
			if (image != null)
			{
				image = Utils.ResizeImage(image, 80, 80);
				_avatarImageRef.Add(userAvatar.AvatarId);
				PictureBox pictureBox = new PictureBox
				{
					Image = image,
					SizeMode = PictureBoxSizeMode.StretchImage,
					Tag = userAvatar.Guid,
					Size = new Size(80, 80),
					Margin = new Padding(8, 4, 3, 3),
					Cursor = Cursors.Hand
				};
				pictureBox.Click += AvatarPictureBox_Click;
				pictureBox.DoubleClick += AvatarPictureBox_DoubleClick;
				tlpAvatars.Controls.Add(pictureBox);
			}
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
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.btnConfirm = new System.Windows.Forms.Button();
		this.groupBox = new System.Windows.Forms.GroupBox();
		this.tlpAvatars = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel1.SuspendLayout();
		this.groupBox.SuspendLayout();
		base.SuspendLayout();
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Controls.Add(this.btnConfirm, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.groupBox, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 761);
		this.tableLayoutPanel1.TabIndex = 0;
		this.btnConfirm.Dock = System.Windows.Forms.DockStyle.Right;
		this.btnConfirm.Location = new System.Drawing.Point(706, 733);
		this.btnConfirm.Name = "btnConfirm";
		this.btnConfirm.Size = new System.Drawing.Size(75, 25);
		this.btnConfirm.TabIndex = 0;
		this.btnConfirm.Text = "Confirm";
		this.btnConfirm.UseVisualStyleBackColor = true;
		this.btnConfirm.Click += new System.EventHandler(btnConfirm_Click);
		this.groupBox.Controls.Add(this.tlpAvatars);
		this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBox.Location = new System.Drawing.Point(3, 3);
		this.groupBox.Name = "groupBox";
		this.groupBox.Size = new System.Drawing.Size(778, 724);
		this.groupBox.TabIndex = 1;
		this.groupBox.TabStop = false;
		this.groupBox.Text = "Role";
		this.tlpAvatars.AutoScroll = true;
		this.tlpAvatars.BackColor = System.Drawing.SystemColors.Window;
		this.tlpAvatars.ColumnCount = 8;
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5f));
		this.tlpAvatars.Location = new System.Drawing.Point(3, 19);
		this.tlpAvatars.Name = "tlpAvatars";
		this.tlpAvatars.RowCount = 12;
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tlpAvatars.Size = new System.Drawing.Size(772, 699);
		this.tlpAvatars.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(784, 761);
		base.Controls.Add(this.tableLayoutPanel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AvatarSelectionForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Select Character";
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(AvatarSelectionForm_FormClosed);
		base.Load += new System.EventHandler(AvatarSelectionForm_Load);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.groupBox.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
