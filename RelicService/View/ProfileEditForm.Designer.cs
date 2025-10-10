namespace RelicService.View
{
    partial class ProfileEditForm
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
            tableLayoutPanel = new TableLayoutPanel();
            labelProfileName = new Label();
            tbProfileName = new TextBox();
            labelSceneId = new Label();
            tbSceneIds = new TextBox();
            btnSave = new Button();
            btnDelete = new Button();
            labelTeam = new Label();
            btnAddTeam = new Button();
            flpTeams = new FlowLayoutPanel();
            cbAutoEquip = new CheckBox();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 4;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81F));
            tableLayoutPanel.Controls.Add(labelProfileName, 0, 0);
            tableLayoutPanel.Controls.Add(tbProfileName, 1, 0);
            tableLayoutPanel.Controls.Add(labelSceneId, 0, 1);
            tableLayoutPanel.Controls.Add(tbSceneIds, 1, 1);
            tableLayoutPanel.Controls.Add(btnSave, 3, 4);
            tableLayoutPanel.Controls.Add(btnDelete, 2, 4);
            tableLayoutPanel.Controls.Add(labelTeam, 0, 2);
            tableLayoutPanel.Controls.Add(btnAddTeam, 1, 2);
            tableLayoutPanel.Controls.Add(flpTeams, 1, 3);
            tableLayoutPanel.Controls.Add(cbAutoEquip, 0, 4);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(5, 5);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 5;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.Size = new Size(294, 431);
            tableLayoutPanel.TabIndex = 0;
            // 
            // labelProfileName
            // 
            labelProfileName.AutoSize = true;
            labelProfileName.Location = new Point(3, 7);
            labelProfileName.Margin = new Padding(3, 7, 3, 3);
            labelProfileName.Name = "labelProfileName";
            labelProfileName.Size = new Size(46, 15);
            labelProfileName.TabIndex = 0;
            labelProfileName.Text = "配置名";
            // 
            // tbProfileName
            // 
            tableLayoutPanel.SetColumnSpan(tbProfileName, 3);
            tbProfileName.Dock = DockStyle.Fill;
            tbProfileName.Location = new Point(55, 3);
            tbProfileName.Name = "tbProfileName";
            tbProfileName.Size = new Size(236, 23);
            tbProfileName.TabIndex = 1;
            // 
            // labelSceneId
            // 
            labelSceneId.AutoSize = true;
            labelSceneId.Location = new Point(3, 36);
            labelSceneId.Margin = new Padding(3, 7, 3, 3);
            labelSceneId.Name = "labelSceneId";
            labelSceneId.Size = new Size(33, 15);
            labelSceneId.TabIndex = 2;
            labelSceneId.Text = "场景";
            // 
            // tbSceneIds
            // 
            tableLayoutPanel.SetColumnSpan(tbSceneIds, 3);
            tbSceneIds.Dock = DockStyle.Fill;
            tbSceneIds.Location = new Point(55, 32);
            tbSceneIds.Name = "tbSceneIds";
            tbSceneIds.Size = new Size(236, 23);
            tbSceneIds.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(216, 404);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 5;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Dock = DockStyle.Right;
            btnDelete.Location = new Point(135, 404);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 24);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "删除";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // labelTeam
            // 
            labelTeam.AutoSize = true;
            labelTeam.Location = new Point(3, 65);
            labelTeam.Margin = new Padding(3, 7, 3, 3);
            labelTeam.Name = "labelTeam";
            labelTeam.Size = new Size(33, 15);
            labelTeam.TabIndex = 4;
            labelTeam.Text = "队伍";
            // 
            // btnAddTeam
            // 
            tableLayoutPanel.SetColumnSpan(btnAddTeam, 3);
            btnAddTeam.Dock = DockStyle.Fill;
            btnAddTeam.Location = new Point(55, 61);
            btnAddTeam.Name = "btnAddTeam";
            btnAddTeam.Size = new Size(236, 23);
            btnAddTeam.TabIndex = 7;
            btnAddTeam.Text = "添加队伍";
            btnAddTeam.UseVisualStyleBackColor = true;
            btnAddTeam.Click += btnAddTeam_Click;
            // 
            // flpTeams
            // 
            flpTeams.AutoScroll = true;
            flpTeams.BackColor = SystemColors.Window;
            flpTeams.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel.SetColumnSpan(flpTeams, 3);
            flpTeams.Dock = DockStyle.Fill;
            flpTeams.FlowDirection = FlowDirection.TopDown;
            flpTeams.Location = new Point(55, 90);
            flpTeams.Name = "flpTeams";
            flpTeams.Size = new Size(236, 308);
            flpTeams.TabIndex = 8;
            flpTeams.WrapContents = false;
            // 
            // cbAutoEquip
            // 
            cbAutoEquip.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(cbAutoEquip, 2);
            cbAutoEquip.Location = new Point(3, 407);
            cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
            cbAutoEquip.Name = "cbAutoEquip";
            cbAutoEquip.Size = new Size(78, 19);
            cbAutoEquip.TabIndex = 9;
            cbAutoEquip.Text = "自动装备";
            cbAutoEquip.UseVisualStyleBackColor = true;
            // 
            // ProfileEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 441);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProfileEditForm";
            Padding = new Padding(5);
            StartPosition = FormStartPosition.CenterParent;
            Text = "编辑配置：";
            FormClosed += ProfileEditForm_FormClosed;
            Load += ProfileEditForm_Load;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Label labelProfileName;
        private TextBox tbProfileName;
        private Label labelSceneId;
        private TextBox tbSceneIds;
        private Label labelTeam;
        private Button btnSave;
        private Button btnDelete;
        private Button btnAddTeam;
        private FlowLayoutPanel flpTeams;
        private CheckBox cbAutoEquip;
    }
}