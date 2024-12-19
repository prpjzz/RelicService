using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using RelicService.Properties;

namespace RelicService.View;

public class AvatarProfileItem : UserControl
{
	private static readonly float WidthToHeightScale = 5f / 18f;

	private bool _isSelected;

	private ulong _avatarGuid;

	private Action<ulong>? _onAnyItemClicked;

	private readonly object _lock = new object();

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private PictureBox pbAvatar;

	private Label labelAvatarName;

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			tableLayoutPanel.BackColor = (_isSelected ? SystemColors.ControlDark : SystemColors.Window);
		}
	}

	public Image AvatarImage
	{
		set
		{
			pbAvatar.Image = value;
		}
	}

	public string AvatarName
	{
		get
		{
			return labelAvatarName.Text;
		}
		set
		{
			labelAvatarName.Text = value;
		}
	}

	public Action<ulong> OnClickCallback
	{
		set
		{
			_onAnyItemClicked = value;
		}
	}

	public AvatarProfileItem()
	{
	}

	public AvatarProfileItem(ulong avatarGuid)
	{
		InitializeComponent();
		_avatarGuid = avatarGuid;
		tableLayoutPanel.Controls.OfType<Control>().ToList().ForEach(delegate(Control c)
		{
			c.Click += delegate
			{
				_onAnyItemClicked?.Invoke(_avatarGuid);
			};
		});
		tableLayoutPanel.Click += delegate
		{
			_onAnyItemClicked?.Invoke(_avatarGuid);
		};
		base.Click += delegate
		{
			_onAnyItemClicked?.Invoke(_avatarGuid);
		};
		pbAvatar.Image = null;
		Size size = base.Size;
		Size size2 = size;
		size2.Height = (int)((float)size.Width * WidthToHeightScale);
		base.Size = size2;
	}

	private void pbAvatar_Resize(object sender, EventArgs e)
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(_lock, ref lockTaken);
			if (lockTaken)
			{
				PictureBox pictureBox = (PictureBox)sender;
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
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.pbAvatar = new System.Windows.Forms.PictureBox();
		this.labelAvatarName = new System.Windows.Forms.Label();
		this.tableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel.ColumnCount = 2;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72f));
		this.tableLayoutPanel.Controls.Add(this.pbAvatar, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.labelAvatarName, 1, 0);
		this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 1;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(270, 75);
		this.tableLayoutPanel.TabIndex = 0;
		this.pbAvatar.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbAvatar.Image = RelicService.Properties.Resources.dot_green;
		this.pbAvatar.Location = new System.Drawing.Point(5, 5);
		this.pbAvatar.Margin = new System.Windows.Forms.Padding(5);
		this.pbAvatar.Name = "pbAvatar";
		this.pbAvatar.Size = new System.Drawing.Size(65, 65);
		this.pbAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbAvatar.TabIndex = 0;
		this.pbAvatar.TabStop = false;
		this.pbAvatar.Resize += new System.EventHandler(pbAvatar_Resize);
		this.labelAvatarName.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelAvatarName.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelAvatarName.Location = new System.Drawing.Point(78, 0);
		this.labelAvatarName.Name = "labelAvatarName";
		this.labelAvatarName.Padding = new System.Windows.Forms.Padding(40, 0, 0, 0);
		this.labelAvatarName.Size = new System.Drawing.Size(189, 75);
		this.labelAvatarName.TabIndex = 1;
		this.labelAvatarName.Text = "label1";
		this.labelAvatarName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel);
		base.Name = "AvatarProfileItem";
		base.Size = new System.Drawing.Size(270, 75);
		this.tableLayoutPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pbAvatar).EndInit();
		base.ResumeLayout(false);
	}
}
