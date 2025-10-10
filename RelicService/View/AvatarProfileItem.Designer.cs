namespace RelicService.View
{
    partial class AvatarProfileItem
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
            tableLayoutPanel = new TableLayoutPanel();
            pbAvatar = new PictureBox();
            labelAvatarName = new Label();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));
            tableLayoutPanel.Controls.Add(pbAvatar, 0, 0);
            tableLayoutPanel.Controls.Add(labelAvatarName, 1, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(270, 75);
            tableLayoutPanel.TabIndex = 0;
            // 
            // pbAvatar
            // 
            pbAvatar.Dock = DockStyle.Fill;
            pbAvatar.Image = Properties.Resources.dot_green;
            pbAvatar.Location = new Point(5, 5);
            pbAvatar.Margin = new Padding(5);
            pbAvatar.Name = "pbAvatar";
            pbAvatar.Size = new Size(65, 65);
            pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAvatar.TabIndex = 0;
            pbAvatar.TabStop = false;
            pbAvatar.Resize += pbAvatar_Resize;
            // 
            // labelAvatarName
            // 
            labelAvatarName.Dock = DockStyle.Fill;
            labelAvatarName.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelAvatarName.Location = new Point(78, 0);
            labelAvatarName.Name = "labelAvatarName";
            labelAvatarName.Padding = new Padding(40, 0, 0, 0);
            labelAvatarName.Size = new Size(189, 75);
            labelAvatarName.TabIndex = 1;
            labelAvatarName.Text = "label1";
            labelAvatarName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // AvatarProfileItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Name = "AvatarProfileItem";
            Size = new Size(270, 75);
            tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbAvatar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox pbAvatar;
        private Label labelAvatarName;
    }
}
