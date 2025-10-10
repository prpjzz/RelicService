namespace RelicService.View
{
    partial class AvatarTeamItem
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
            contextMenu = new ContextMenuStrip(components);
            menuDelete = new ToolStripMenuItem();
            pbAvatar3 = new PictureBox();
            pbAvatar2 = new PictureBox();
            pbAvatar1 = new PictureBox();
            tableLayoutPanel.SuspendLayout();
            contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbAvatar2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbAvatar1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.ContextMenuStrip = contextMenu;
            tableLayoutPanel.Controls.Add(pbAvatar3, 2, 0);
            tableLayoutPanel.Controls.Add(pbAvatar2, 1, 0);
            tableLayoutPanel.Controls.Add(pbAvatar1, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(230, 71);
            tableLayoutPanel.TabIndex = 0;
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[] { menuDelete });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(181, 48);
            // 
            // menuDelete
            // 
            menuDelete.Name = "menuDelete";
            menuDelete.Size = new Size(180, 22);
            menuDelete.Text = "删除";
            menuDelete.Click += menuDelete_Click;
            // 
            // pbAvatar3
            // 
            pbAvatar3.ContextMenuStrip = contextMenu;
            pbAvatar3.Location = new Point(158, 3);
            pbAvatar3.Margin = new Padding(6, 3, 3, 3);
            pbAvatar3.Name = "pbAvatar3";
            pbAvatar3.Size = new Size(65, 65);
            pbAvatar3.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAvatar3.TabIndex = 2;
            pbAvatar3.TabStop = false;
            // 
            // pbAvatar2
            // 
            pbAvatar2.ContextMenuStrip = contextMenu;
            pbAvatar2.Location = new Point(82, 3);
            pbAvatar2.Margin = new Padding(6, 3, 3, 3);
            pbAvatar2.Name = "pbAvatar2";
            pbAvatar2.Size = new Size(65, 65);
            pbAvatar2.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAvatar2.TabIndex = 1;
            pbAvatar2.TabStop = false;
            // 
            // pbAvatar1
            // 
            pbAvatar1.ContextMenuStrip = contextMenu;
            pbAvatar1.Location = new Point(6, 3);
            pbAvatar1.Margin = new Padding(6, 3, 3, 3);
            pbAvatar1.Name = "pbAvatar1";
            pbAvatar1.Size = new Size(65, 65);
            pbAvatar1.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAvatar1.TabIndex = 0;
            pbAvatar1.TabStop = false;
            // 
            // AvatarTeamItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Name = "AvatarTeamItem";
            Size = new Size(230, 71);
            tableLayoutPanel.ResumeLayout(false);
            contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbAvatar3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbAvatar2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbAvatar1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox pbAvatar1;
        private PictureBox pbAvatar3;
        private PictureBox pbAvatar2;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem menuDelete;
    }
}
