using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RelicService.View;

public class AvatarTeamItem : UserControl
{
	private const float WidthToHeightScale = 0.30869564f;

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private PictureBox pbAvatar1;

	private PictureBox pbAvatar3;

	private PictureBox pbAvatar2;

	private ContextMenuStrip contextMenu;

	private ToolStripMenuItem menuDelete;

	public int ControlIndex { get; set; }

	public Action<AvatarTeamItem, int>? OnDeleteCallback { get; set; }

	public Image AvatarImage1
	{
		set
		{
			pbAvatar1.Image = value;
		}
	}

	public Image AvatarImage2
	{
		set
		{
			pbAvatar2.Image = value;
		}
	}

	public Image AvatarImage3
	{
		set
		{
			pbAvatar3.Image = value;
		}
	}

	public AvatarTeamItem()
	{
		InitializeComponent();
		Size size = base.Size;
		Size size2 = size;
		size2.Height = (int)((float)size.Width * 0.30869564f);
		base.Size = size2;
	}

	private void menuDelete_Click(object sender, EventArgs e)
	{
		OnDeleteCallback?.Invoke(this, ControlIndex);
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
		this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.pbAvatar3 = new System.Windows.Forms.PictureBox();
		this.pbAvatar2 = new System.Windows.Forms.PictureBox();
		this.pbAvatar1 = new System.Windows.Forms.PictureBox();
		this.tableLayoutPanel.SuspendLayout();
		this.contextMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar1).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel.ColumnCount = 3;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.333332f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.333332f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.333332f));
		this.tableLayoutPanel.ContextMenuStrip = this.contextMenu;
		this.tableLayoutPanel.Controls.Add(this.pbAvatar3, 2, 0);
		this.tableLayoutPanel.Controls.Add(this.pbAvatar2, 1, 0);
		this.tableLayoutPanel.Controls.Add(this.pbAvatar1, 0, 0);
		this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 1;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(230, 71);
		this.tableLayoutPanel.TabIndex = 0;
		this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menuDelete });
		this.contextMenu.Name = "contextMenu";
		this.contextMenu.Size = new System.Drawing.Size(181, 48);
		this.menuDelete.Name = "menuDelete";
		this.menuDelete.Size = new System.Drawing.Size(180, 22);
		this.menuDelete.Text = "Delete";
		this.menuDelete.Click += new System.EventHandler(menuDelete_Click);
		this.pbAvatar3.ContextMenuStrip = this.contextMenu;
		this.pbAvatar3.Location = new System.Drawing.Point(158, 3);
		this.pbAvatar3.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
		this.pbAvatar3.Name = "pbAvatar3";
		this.pbAvatar3.Size = new System.Drawing.Size(65, 65);
		this.pbAvatar3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbAvatar3.TabIndex = 2;
		this.pbAvatar3.TabStop = false;
		this.pbAvatar2.ContextMenuStrip = this.contextMenu;
		this.pbAvatar2.Location = new System.Drawing.Point(82, 3);
		this.pbAvatar2.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
		this.pbAvatar2.Name = "pbAvatar2";
		this.pbAvatar2.Size = new System.Drawing.Size(65, 65);
		this.pbAvatar2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbAvatar2.TabIndex = 1;
		this.pbAvatar2.TabStop = false;
		this.pbAvatar1.ContextMenuStrip = this.contextMenu;
		this.pbAvatar1.Location = new System.Drawing.Point(6, 3);
		this.pbAvatar1.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
		this.pbAvatar1.Name = "pbAvatar1";
		this.pbAvatar1.Size = new System.Drawing.Size(65, 65);
		this.pbAvatar1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbAvatar1.TabIndex = 0;
		this.pbAvatar1.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel);
		base.Name = "AvatarTeamItem";
		base.Size = new System.Drawing.Size(230, 71);
		this.tableLayoutPanel.ResumeLayout(false);
		this.contextMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pbAvatar3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbAvatar1).EndInit();
		base.ResumeLayout(false);
	}
}
