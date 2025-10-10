namespace RelicService.View
{
    partial class AvatarSelectionForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            btnConfirm = new Button();
            groupBox = new GroupBox();
            tlpAvatars = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            groupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(btnConfirm, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBox, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 96F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4F));
            tableLayoutPanel1.Size = new Size(784, 761);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // btnConfirm
            // 
            btnConfirm.Dock = DockStyle.Right;
            btnConfirm.Location = new Point(706, 733);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(75, 25);
            btnConfirm.TabIndex = 0;
            btnConfirm.Text = "确定";
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // groupBox
            // 
            groupBox.Controls.Add(tlpAvatars);
            groupBox.Dock = DockStyle.Fill;
            groupBox.Location = new Point(3, 3);
            groupBox.Name = "groupBox";
            groupBox.Size = new Size(778, 724);
            groupBox.TabIndex = 1;
            groupBox.TabStop = false;
            groupBox.Text = "角色";
            // 
            // tlpAvatars
            // 
            tlpAvatars.AutoScroll = true;
            tlpAvatars.BackColor = SystemColors.Window;
            tlpAvatars.ColumnCount = 8;
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            tlpAvatars.Location = new Point(3, 19);
            tlpAvatars.Name = "tlpAvatars";
            tlpAvatars.RowCount = 12;
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.RowStyles.Add(new RowStyle());
            tlpAvatars.Size = new Size(772, 699);
            tlpAvatars.TabIndex = 0;
            // 
            // AvatarSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 761);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AvatarSelectionForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "角色选择";
            FormClosed += AvatarSelectionForm_FormClosed;
            Load += AvatarSelectionForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            groupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button btnConfirm;
        private GroupBox groupBox;
        private TableLayoutPanel tlpAvatars;
    }
}