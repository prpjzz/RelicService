namespace RelicService.View
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            statusStrip = new StatusStrip();
            serverStatus = new ToolStripStatusLabel();
            menuStrip = new MenuStrip();
            menuExtra = new ToolStripMenuItem();
            tsOpenCrafting = new ToolStripMenuItem();
            menuAbout = new ToolStripMenuItem();
            groupBox = new GroupBox();
            splitContainer = new SplitContainer();
            tableLayoutLeft = new TableLayoutPanel();
            groupBoxAvatar = new GroupBox();
            flpAvatars = new FlowLayoutPanel();
            btnAddPreset = new Button();
            textBoxAvatarFilter = new TextBox();
            tableLayoutPanel = new TableLayoutPanel();
            groupBoxSelectedAvatar = new GroupBox();
            flpProfileDetails = new FlowLayoutPanel();
            labelUid = new Label();
            labelScene = new Label();
            cbAutoEquip = new CheckBox();
            labelConflict = new Label();
            btnConfirmConflict = new Button();
            cbEnableGameMessage = new CheckBox();
            btnRefreshMetadata = new Button();
            statusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutLeft.SuspendLayout();
            groupBoxAvatar.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            groupBoxSelectedAvatar.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { serverStatus });
            statusStrip.Location = new Point(0, 659);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1264, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip";
            // 
            // serverStatus
            // 
            serverStatus.Image = Properties.Resources.dot_red;
            serverStatus.Name = "serverStatus";
            serverStatus.Size = new Size(75, 17);
            serverStatus.Text = "服务离线";
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuExtra, menuAbout });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1264, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip";
            // 
            // menuExtra
            // 
            menuExtra.DropDownItems.AddRange(new ToolStripItem[] { tsOpenCrafting });
            menuExtra.Name = "menuExtra";
            menuExtra.Size = new Size(45, 20);
            menuExtra.Text = "更多";
            // 
            // tsOpenCrafting
            // 
            tsOpenCrafting.Name = "tsOpenCrafting";
            tsOpenCrafting.Size = new Size(139, 22);
            tsOpenCrafting.Text = "打开合成台";
            tsOpenCrafting.Click += tsOpenCrafting_Click;
            // 
            // menuAbout
            // 
            menuAbout.Name = "menuAbout";
            menuAbout.Size = new Size(45, 20);
            menuAbout.Text = "关于";
            menuAbout.Click += menuAbout_Click;
            // 
            // groupBox
            // 
            groupBox.Controls.Add(splitContainer);
            groupBox.Dock = DockStyle.Fill;
            groupBox.Location = new Point(0, 24);
            groupBox.Name = "groupBox";
            groupBox.Size = new Size(1264, 635);
            groupBox.TabIndex = 2;
            groupBox.TabStop = false;
            groupBox.Text = "主页";
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(3, 19);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutLeft);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(tableLayoutPanel);
            splitContainer.Size = new Size(1258, 613);
            splitContainer.SplitterDistance = 350;
            splitContainer.TabIndex = 0;
            // 
            // tableLayoutLeft
            // 
            tableLayoutLeft.ColumnCount = 1;
            tableLayoutLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutLeft.Controls.Add(groupBoxAvatar, 0, 2);
            tableLayoutLeft.Controls.Add(btnAddPreset, 0, 0);
            tableLayoutLeft.Controls.Add(textBoxAvatarFilter, 0, 1);
            tableLayoutLeft.Dock = DockStyle.Fill;
            tableLayoutLeft.Location = new Point(0, 0);
            tableLayoutLeft.Name = "tableLayoutLeft";
            tableLayoutLeft.RowCount = 3;
            tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            tableLayoutLeft.Size = new Size(350, 613);
            tableLayoutLeft.TabIndex = 0;
            // 
            // groupBoxAvatar
            // 
            groupBoxAvatar.Controls.Add(flpAvatars);
            groupBoxAvatar.Dock = DockStyle.Fill;
            groupBoxAvatar.Location = new Point(3, 63);
            groupBoxAvatar.Name = "groupBoxAvatar";
            groupBoxAvatar.Size = new Size(344, 547);
            groupBoxAvatar.TabIndex = 1;
            groupBoxAvatar.TabStop = false;
            groupBoxAvatar.Text = "预设角色列表";
            // 
            // flpAvatars
            // 
            flpAvatars.AutoScroll = true;
            flpAvatars.BackColor = SystemColors.ControlLightLight;
            flpAvatars.BorderStyle = BorderStyle.FixedSingle;
            flpAvatars.Dock = DockStyle.Fill;
            flpAvatars.Location = new Point(3, 19);
            flpAvatars.Name = "flpAvatars";
            flpAvatars.Padding = new Padding(18, 10, 20, 20);
            flpAvatars.Size = new Size(338, 525);
            flpAvatars.TabIndex = 0;
            // 
            // btnAddPreset
            // 
            btnAddPreset.Dock = DockStyle.Fill;
            btnAddPreset.Location = new Point(5, 3);
            btnAddPreset.Margin = new Padding(5, 3, 5, 3);
            btnAddPreset.Name = "btnAddPreset";
            btnAddPreset.Size = new Size(340, 24);
            btnAddPreset.TabIndex = 0;
            btnAddPreset.Text = "添加预设";
            btnAddPreset.UseVisualStyleBackColor = true;
            btnAddPreset.Click += btnAddPreset_Click;
            // 
            // textBoxAvatarFilter
            // 
            textBoxAvatarFilter.Dock = DockStyle.Fill;
            textBoxAvatarFilter.Location = new Point(6, 33);
            textBoxAvatarFilter.Margin = new Padding(6, 3, 6, 3);
            textBoxAvatarFilter.Name = "textBoxAvatarFilter";
            textBoxAvatarFilter.PlaceholderText = "列表过滤";
            textBoxAvatarFilter.Size = new Size(338, 23);
            textBoxAvatarFilter.TabIndex = 2;
            textBoxAvatarFilter.TextChanged += textBoxAvatarFilter_TextChanged;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 5;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel.Controls.Add(groupBoxSelectedAvatar, 0, 2);
            tableLayoutPanel.Controls.Add(labelUid, 0, 0);
            tableLayoutPanel.Controls.Add(labelScene, 0, 1);
            tableLayoutPanel.Controls.Add(cbAutoEquip, 1, 1);
            tableLayoutPanel.Controls.Add(labelConflict, 1, 0);
            tableLayoutPanel.Controls.Add(btnConfirmConflict, 4, 0);
            tableLayoutPanel.Controls.Add(cbEnableGameMessage, 2, 1);
            tableLayoutPanel.Controls.Add(btnRefreshMetadata, 3, 1);
            tableLayoutPanel.Dock = DockStyle.Top;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            tableLayoutPanel.Size = new Size(904, 613);
            tableLayoutPanel.TabIndex = 1;
            // 
            // groupBoxSelectedAvatar
            // 
            tableLayoutPanel.SetColumnSpan(groupBoxSelectedAvatar, 5);
            groupBoxSelectedAvatar.Controls.Add(flpProfileDetails);
            groupBoxSelectedAvatar.Dock = DockStyle.Fill;
            groupBoxSelectedAvatar.Location = new Point(3, 63);
            groupBoxSelectedAvatar.Name = "groupBoxSelectedAvatar";
            groupBoxSelectedAvatar.Size = new Size(898, 547);
            groupBoxSelectedAvatar.TabIndex = 0;
            groupBoxSelectedAvatar.TabStop = false;
            groupBoxSelectedAvatar.Text = "<角色名>";
            // 
            // flpProfileDetails
            // 
            flpProfileDetails.AutoScroll = true;
            flpProfileDetails.BackColor = SystemColors.Window;
            flpProfileDetails.BorderStyle = BorderStyle.FixedSingle;
            flpProfileDetails.Dock = DockStyle.Fill;
            flpProfileDetails.FlowDirection = FlowDirection.TopDown;
            flpProfileDetails.Location = new Point(3, 19);
            flpProfileDetails.Name = "flpProfileDetails";
            flpProfileDetails.Size = new Size(892, 525);
            flpProfileDetails.TabIndex = 0;
            flpProfileDetails.WrapContents = false;
            // 
            // labelUid
            // 
            labelUid.AutoSize = true;
            labelUid.Location = new Point(2, 7);
            labelUid.Margin = new Padding(2, 7, 7, 7);
            labelUid.Name = "labelUid";
            labelUid.Size = new Size(32, 15);
            labelUid.TabIndex = 1;
            labelUid.Text = "UID: ";
            // 
            // labelScene
            // 
            labelScene.AutoSize = true;
            labelScene.Location = new Point(2, 37);
            labelScene.Margin = new Padding(2, 7, 7, 7);
            labelScene.Name = "labelScene";
            labelScene.Size = new Size(36, 15);
            labelScene.TabIndex = 3;
            labelScene.Text = "场景:";
            // 
            // cbAutoEquip
            // 
            cbAutoEquip.AutoSize = true;
            cbAutoEquip.Location = new Point(48, 36);
            cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
            cbAutoEquip.Name = "cbAutoEquip";
            cbAutoEquip.Size = new Size(78, 19);
            cbAutoEquip.TabIndex = 4;
            cbAutoEquip.Text = "自动装备";
            cbAutoEquip.UseVisualStyleBackColor = true;
            // 
            // labelConflict
            // 
            labelConflict.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(labelConflict, 3);
            labelConflict.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelConflict.ForeColor = Color.DarkRed;
            labelConflict.Location = new Point(47, 7);
            labelConflict.Margin = new Padding(2, 7, 7, 7);
            labelConflict.Name = "labelConflict";
            labelConflict.Size = new Size(40, 15);
            labelConflict.TabIndex = 5;
            labelConflict.Text = "label1";
            // 
            // btnConfirmConflict
            // 
            btnConfirmConflict.Location = new Point(827, 3);
            btnConfirmConflict.Name = "btnConfirmConflict";
            btnConfirmConflict.Size = new Size(74, 23);
            btnConfirmConflict.TabIndex = 6;
            btnConfirmConflict.Text = "确认";
            btnConfirmConflict.UseVisualStyleBackColor = true;
            btnConfirmConflict.Click += btnConfirmConflict_Click;
            // 
            // cbEnableGameMessage
            // 
            cbEnableGameMessage.AutoSize = true;
            cbEnableGameMessage.Location = new Point(132, 36);
            cbEnableGameMessage.Margin = new Padding(3, 6, 3, 3);
            cbEnableGameMessage.Name = "cbEnableGameMessage";
            cbEnableGameMessage.Size = new Size(117, 19);
            cbEnableGameMessage.TabIndex = 7;
            cbEnableGameMessage.Text = "启用游戏内通知";
            cbEnableGameMessage.UseVisualStyleBackColor = true;
            // 
            // btnRefreshMetadata
            // 
            btnRefreshMetadata.AutoSize = true;
            btnRefreshMetadata.Location = new Point(255, 33);
            btnRefreshMetadata.Name = "btnRefreshMetadata";
            btnRefreshMetadata.Size = new Size(82, 24);
            btnRefreshMetadata.TabIndex = 8;
            btnRefreshMetadata.Text = "刷新元数据";
            btnRefreshMetadata.UseVisualStyleBackColor = true;
            btnRefreshMetadata.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(groupBox);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "圣遗物预设系统 by: Ex_M";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            groupBox.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutLeft.ResumeLayout(false);
            tableLayoutLeft.PerformLayout();
            groupBoxAvatar.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            groupBoxSelectedAvatar.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip;
        private ToolStripStatusLabel serverStatus;
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuAbout;
        private GroupBox groupBox;
        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutLeft;
        private Button btnAddPreset;
        private GroupBox groupBoxAvatar;
        private FlowLayoutPanel flpAvatars;
        private GroupBox groupBoxSelectedAvatar;
        private TextBox textBoxAvatarFilter;
        private TableLayoutPanel tableLayoutPanel;
        private Label labelUid;
        private Label labelScene;
        private CheckBox cbAutoEquip;
        private Label labelConflict;
        private Button btnConfirmConflict;
        private FlowLayoutPanel flpProfileDetails;
        private CheckBox cbEnableGameMessage;
        private Button btnRefreshMetadata;
        private ToolStripMenuItem menuExtra;
        private ToolStripMenuItem tsOpenCrafting;
    }
}
