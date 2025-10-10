namespace RelicService.View
{
    partial class AvatarRelicInfo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanel = new TableLayoutPanel();
            pbAvatar = new PictureBox();
            pbFlower = new PictureBox();
            pbFeather = new PictureBox();
            pbSand = new PictureBox();
            pbGoblet = new PictureBox();
            pbHead = new PictureBox();
            toolTip = new ToolTip(components);
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbFlower).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbFeather).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbGoblet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbHead).BeginInit();
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
            pbAvatar.Image = Properties.Resources.dot_green;
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
            pbFlower.Image = Properties.Resources.dot_red;
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
            pbFeather.Image = Properties.Resources.dot_yellow;
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
            pbSand.Image = Properties.Resources.dot_green;
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
            pbGoblet.Image = Properties.Resources.dot_red;
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
            pbHead.Image = Properties.Resources.dot_yellow;
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
            ((System.ComponentModel.ISupportInitialize)pbAvatar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbFlower).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbFeather).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSand).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbGoblet).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbHead).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox pbAvatar;
        private PictureBox pbFlower;
        private PictureBox pbFeather;
        private PictureBox pbSand;
        private PictureBox pbGoblet;
        private PictureBox pbHead;
        private ToolTip toolTip;
    }
}
