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
        components = new Container();
        ComponentResourceManager resources = new ComponentResourceManager(typeof(AvatarRelicInfo));
        tableLayoutPanel = new TableLayoutPanel();
        pbAvatar = new PictureBox();
        pbFlower = new PictureBox();
        pbFeather = new PictureBox();
        pbSand = new PictureBox();
        pbGoblet = new PictureBox();
        pbHead = new PictureBox();
        toolTip = new ToolTip(components);
        tableLayoutPanel.SuspendLayout();
        ((ISupportInitialize)pbAvatar).BeginInit();
        ((ISupportInitialize)pbFlower).BeginInit();
        ((ISupportInitialize)pbFeather).BeginInit();
        ((ISupportInitialize)pbSand).BeginInit();
        ((ISupportInitialize)pbGoblet).BeginInit();
        ((ISupportInitialize)pbHead).BeginInit();
        SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.BackColor = SystemColors.Menu;
        tableLayoutPanel.ColumnCount = 6;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanel.Controls.Add(pbAvatar, 0, 0);
        tableLayoutPanel.Controls.Add(pbFlower, 1, 0);
        tableLayoutPanel.Controls.Add(pbFeather, 2, 0);
        tableLayoutPanel.Controls.Add(pbSand, 3, 0);
        tableLayoutPanel.Controls.Add(pbGoblet, 4, 0);
        tableLayoutPanel.Controls.Add(pbHead, 5, 0);
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 1;
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Size = new Size(900, 150);
        tableLayoutPanel.TabIndex = 0;
        // 
        // pbAvatar
        // 
        pbAvatar.Dock = DockStyle.Fill;
        pbAvatar.Image = (Image)resources.GetObject("pbAvatar.Image");
        pbAvatar.Location = new Point(5, 5);
        pbAvatar.Margin = new Padding(5);
        pbAvatar.Name = "pbAvatar";
        pbAvatar.Size = new Size(140, 140);
        pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
        pbAvatar.TabIndex = 0;
        pbAvatar.TabStop = false;
        // 
        // pbFlower
        // 
        pbFlower.Dock = DockStyle.Fill;
        pbFlower.Image = (Image)resources.GetObject("pbFlower.Image");
        pbFlower.Location = new Point(155, 5);
        pbFlower.Margin = new Padding(5);
        pbFlower.Name = "pbFlower";
        pbFlower.Size = new Size(140, 140);
        pbFlower.SizeMode = PictureBoxSizeMode.StretchImage;
        pbFlower.TabIndex = 1;
        pbFlower.TabStop = false;
        // 
        // pbFeather
        // 
        pbFeather.Dock = DockStyle.Fill;
        pbFeather.Image = (Image)resources.GetObject("pbFeather.Image");
        pbFeather.Location = new Point(305, 5);
        pbFeather.Margin = new Padding(5);
        pbFeather.Name = "pbFeather";
        pbFeather.Size = new Size(140, 140);
        pbFeather.SizeMode = PictureBoxSizeMode.StretchImage;
        pbFeather.TabIndex = 2;
        pbFeather.TabStop = false;
        // 
        // pbSand
        // 
        pbSand.Dock = DockStyle.Fill;
        pbSand.Image = (Image)resources.GetObject("pbSand.Image");
        pbSand.Location = new Point(455, 5);
        pbSand.Margin = new Padding(5);
        pbSand.Name = "pbSand";
        pbSand.Size = new Size(140, 140);
        pbSand.SizeMode = PictureBoxSizeMode.StretchImage;
        pbSand.TabIndex = 3;
        pbSand.TabStop = false;
        // 
        // pbGoblet
        // 
        pbGoblet.Dock = DockStyle.Fill;
        pbGoblet.Image = (Image)resources.GetObject("pbGoblet.Image");
        pbGoblet.Location = new Point(605, 5);
        pbGoblet.Margin = new Padding(5);
        pbGoblet.Name = "pbGoblet";
        pbGoblet.Size = new Size(140, 140);
        pbGoblet.SizeMode = PictureBoxSizeMode.StretchImage;
        pbGoblet.TabIndex = 4;
        pbGoblet.TabStop = false;
        // 
        // pbHead
        // 
        pbHead.Dock = DockStyle.Fill;
        pbHead.Image = (Image)resources.GetObject("pbHead.Image");
        pbHead.Location = new Point(755, 5);
        pbHead.Margin = new Padding(5);
        pbHead.Name = "pbHead";
        pbHead.Size = new Size(140, 140);
        pbHead.SizeMode = PictureBoxSizeMode.StretchImage;
        pbHead.TabIndex = 5;
        pbHead.TabStop = false;
        // 
        // toolTip
        // 
        toolTip.AutomaticDelay = 0;
        toolTip.AutoPopDelay = 15000;
        toolTip.InitialDelay = 0;
        toolTip.ReshowDelay = 0;
        toolTip.UseAnimation = false;
        toolTip.UseFading = false;
        // 
        // AvatarRelicInfo
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        Controls.Add(tableLayoutPanel);
        Name = "AvatarRelicInfo";
        Size = new Size(900, 150);
        tableLayoutPanel.ResumeLayout(false);
        ((ISupportInitialize)pbAvatar).EndInit();
        ((ISupportInitialize)pbFlower).EndInit();
        ((ISupportInitialize)pbFeather).EndInit();
        ((ISupportInitialize)pbSand).EndInit();
        ((ISupportInitialize)pbGoblet).EndInit();
        ((ISupportInitialize)pbHead).EndInit();
        ResumeLayout(false);
    }
}
