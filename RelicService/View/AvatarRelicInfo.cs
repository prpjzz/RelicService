using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RelicService.Properties;

namespace RelicService.View;

public class AvatarRelicInfo : UserControl
{
	private bool _isSelected;

	private ulong _avatarGuid;

	private Action<ulong>? _onAnyItemClicked;

	private IContainer components;

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
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			tableLayoutPanel.BackColor = (_isSelected ? SystemColors.ControlDark : SystemColors.Menu);
		}
	}

	public Image AvatarImage
	{
		set
		{
			pbAvatar.Image = value;
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

	public string FlowerTooltip
	{
		set
		{
			toolTip.SetToolTip(pbFlower, value);
		}
	}

	public string FeatherTooltip
	{
		set
		{
			toolTip.SetToolTip(pbFeather, value);
		}
	}

	public string SandTooltip
	{
		set
		{
			toolTip.SetToolTip(pbSand, value);
		}
	}

	public string GobletTooltip
	{
		set
		{
			toolTip.SetToolTip(pbGoblet, value);
		}
	}

	public string HeadTooltip
	{
		set
		{
			toolTip.SetToolTip(pbHead, value);
		}
	}

	public Action<ulong> OnClickCallback
	{
		set
		{
			_onAnyItemClicked = value;
		}
	}

	public AvatarRelicInfo(ulong avatarGuid)
	{
		InitializeComponent();
		_avatarGuid = avatarGuid;
		tableLayoutPanel.Controls.OfType<PictureBox>().ToList().ForEach(delegate(PictureBox pb)
		{
			pb.Image = null;
			pb.Click += delegate
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
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.pbAvatar = new System.Windows.Forms.PictureBox();
		this.pbFlower = new System.Windows.Forms.PictureBox();
		this.pbFeather = new System.Windows.Forms.PictureBox();
		this.pbSand = new System.Windows.Forms.PictureBox();
		this.pbGoblet = new System.Windows.Forms.PictureBox();
		this.pbHead = new System.Windows.Forms.PictureBox();
		this.toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.tableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbFlower).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbFeather).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbSand).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbGoblet).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbHead).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Menu;
		this.tableLayoutPanel.ColumnCount = 6;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel.Controls.Add(this.pbAvatar, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.pbFlower, 1, 0);
		this.tableLayoutPanel.Controls.Add(this.pbFeather, 2, 0);
		this.tableLayoutPanel.Controls.Add(this.pbSand, 3, 0);
		this.tableLayoutPanel.Controls.Add(this.pbGoblet, 4, 0);
		this.tableLayoutPanel.Controls.Add(this.pbHead, 5, 0);
		this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 1;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(900, 150);
		this.tableLayoutPanel.TabIndex = 0;
		this.pbAvatar.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbAvatar.Image = RelicService.Properties.Resources.dot_green;
		this.pbAvatar.Location = new System.Drawing.Point(5, 5);
		this.pbAvatar.Margin = new System.Windows.Forms.Padding(5);
		this.pbAvatar.Name = "pbAvatar";
		this.pbAvatar.Size = new System.Drawing.Size(140, 140);
		this.pbAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbAvatar.TabIndex = 0;
		this.pbAvatar.TabStop = false;
		this.pbFlower.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbFlower.Image = RelicService.Properties.Resources.dot_red;
		this.pbFlower.Location = new System.Drawing.Point(155, 5);
		this.pbFlower.Margin = new System.Windows.Forms.Padding(5);
		this.pbFlower.Name = "pbFlower";
		this.pbFlower.Size = new System.Drawing.Size(140, 140);
		this.pbFlower.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbFlower.TabIndex = 1;
		this.pbFlower.TabStop = false;
		this.pbFeather.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbFeather.Image = RelicService.Properties.Resources.dot_yellow;
		this.pbFeather.Location = new System.Drawing.Point(305, 5);
		this.pbFeather.Margin = new System.Windows.Forms.Padding(5);
		this.pbFeather.Name = "pbFeather";
		this.pbFeather.Size = new System.Drawing.Size(140, 140);
		this.pbFeather.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbFeather.TabIndex = 2;
		this.pbFeather.TabStop = false;
		this.pbSand.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbSand.Image = RelicService.Properties.Resources.dot_green;
		this.pbSand.Location = new System.Drawing.Point(455, 5);
		this.pbSand.Margin = new System.Windows.Forms.Padding(5);
		this.pbSand.Name = "pbSand";
		this.pbSand.Size = new System.Drawing.Size(140, 140);
		this.pbSand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbSand.TabIndex = 3;
		this.pbSand.TabStop = false;
		this.pbGoblet.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbGoblet.Image = RelicService.Properties.Resources.dot_red;
		this.pbGoblet.Location = new System.Drawing.Point(605, 5);
		this.pbGoblet.Margin = new System.Windows.Forms.Padding(5);
		this.pbGoblet.Name = "pbGoblet";
		this.pbGoblet.Size = new System.Drawing.Size(140, 140);
		this.pbGoblet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbGoblet.TabIndex = 4;
		this.pbGoblet.TabStop = false;
		this.pbHead.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pbHead.Image = RelicService.Properties.Resources.dot_yellow;
		this.pbHead.Location = new System.Drawing.Point(755, 5);
		this.pbHead.Margin = new System.Windows.Forms.Padding(5);
		this.pbHead.Name = "pbHead";
		this.pbHead.Size = new System.Drawing.Size(140, 140);
		this.pbHead.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbHead.TabIndex = 5;
		this.pbHead.TabStop = false;
		this.toolTip.AutomaticDelay = 0;
		this.toolTip.AutoPopDelay = 15000;
		this.toolTip.InitialDelay = 0;
		this.toolTip.ReshowDelay = 0;
		this.toolTip.UseAnimation = false;
		this.toolTip.UseFading = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.Controls.Add(this.tableLayoutPanel);
		base.Name = "AvatarRelicInfo";
		base.Size = new System.Drawing.Size(900, 150);
		this.tableLayoutPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pbAvatar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbFlower).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbFeather).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbSand).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbGoblet).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbHead).EndInit();
		base.ResumeLayout(false);
	}
}
