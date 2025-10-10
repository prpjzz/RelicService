namespace RelicService.View
{
    partial class AddPresetForm
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
            splitContainer = new SplitContainer();
            tabControl = new TabControl();
            tabCurrentTeam = new TabPage();
            btnRefreshTeam = new Button();
            flpCurrentTeam = new FlowLayoutPanel();
            tabAllAvatar = new TabPage();
            btnRefreshAllAvatar = new Button();
            flpAllAvatar = new FlowLayoutPanel();
            groupBoxRight = new GroupBox();
            tableLayoutPanel = new TableLayoutPanel();
            textBoxProfileName = new TextBox();
            label1 = new Label();
            groupBoxAutoEquip = new GroupBox();
            tableLayoutAutoEquip = new TableLayoutPanel();
            cbEnableAutoEquip = new CheckBox();
            cbActiveWithScene = new CheckBox();
            labelSceneId = new Label();
            tbSceneIds = new TextBox();
            cbActiveWithTeam = new CheckBox();
            labelTeammates = new Label();
            labelSelectedTeammates = new Label();
            tbTeammateSearch = new TextBox();
            btnUseCurrentTeam = new Button();
            flpAvatars = new FlowLayoutPanel();
            btnClearSelectedTeam = new Button();
            btnSave = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tabControl.SuspendLayout();
            tabCurrentTeam.SuspendLayout();
            tabAllAvatar.SuspendLayout();
            groupBoxRight.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            groupBoxAutoEquip.SuspendLayout();
            tableLayoutAutoEquip.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 3);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tabControl);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(groupBoxRight);
            splitContainer.Size = new Size(800, 422);
            splitContainer.SplitterDistance = 500;
            splitContainer.TabIndex = 0;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabCurrentTeam);
            tabControl.Controls.Add(tabAllAvatar);
            tabControl.Location = new Point(5, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(495, 419);
            tabControl.TabIndex = 0;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // tabCurrentTeam
            // 
            tabCurrentTeam.Controls.Add(btnRefreshTeam);
            tabCurrentTeam.Controls.Add(flpCurrentTeam);
            tabCurrentTeam.Location = new Point(4, 24);
            tabCurrentTeam.Name = "tabCurrentTeam";
            tabCurrentTeam.Padding = new Padding(3);
            tabCurrentTeam.Size = new Size(487, 391);
            tabCurrentTeam.TabIndex = 0;
            tabCurrentTeam.Text = "当前队伍";
            tabCurrentTeam.UseVisualStyleBackColor = true;
            // 
            // btnRefreshTeam
            // 
            btnRefreshTeam.Dock = DockStyle.Bottom;
            btnRefreshTeam.Location = new Point(3, 365);
            btnRefreshTeam.Name = "btnRefreshTeam";
            btnRefreshTeam.Size = new Size(481, 23);
            btnRefreshTeam.TabIndex = 1;
            btnRefreshTeam.Text = "刷新队伍";
            btnRefreshTeam.UseVisualStyleBackColor = true;
            btnRefreshTeam.Click += btnRefreshTeam_Click;
            // 
            // flpCurrentTeam
            // 
            flpCurrentTeam.AutoScroll = true;
            flpCurrentTeam.Dock = DockStyle.Top;
            flpCurrentTeam.FlowDirection = FlowDirection.TopDown;
            flpCurrentTeam.Location = new Point(3, 3);
            flpCurrentTeam.Name = "flpCurrentTeam";
            flpCurrentTeam.Size = new Size(481, 360);
            flpCurrentTeam.TabIndex = 0;
            flpCurrentTeam.WrapContents = false;
            // 
            // tabAllAvatar
            // 
            tabAllAvatar.Controls.Add(btnRefreshAllAvatar);
            tabAllAvatar.Controls.Add(flpAllAvatar);
            tabAllAvatar.Location = new Point(4, 24);
            tabAllAvatar.Name = "tabAllAvatar";
            tabAllAvatar.Padding = new Padding(3);
            tabAllAvatar.Size = new Size(487, 391);
            tabAllAvatar.TabIndex = 1;
            tabAllAvatar.Text = "所有人物";
            tabAllAvatar.UseVisualStyleBackColor = true;
            // 
            // btnRefreshAllAvatar
            // 
            btnRefreshAllAvatar.Dock = DockStyle.Bottom;
            btnRefreshAllAvatar.Location = new Point(3, 365);
            btnRefreshAllAvatar.Name = "btnRefreshAllAvatar";
            btnRefreshAllAvatar.Size = new Size(481, 23);
            btnRefreshAllAvatar.TabIndex = 1;
            btnRefreshAllAvatar.Text = "刷新列表";
            btnRefreshAllAvatar.UseVisualStyleBackColor = true;
            btnRefreshAllAvatar.Click += btnRefreshAllAvatar_Click;
            // 
            // flpAllAvatar
            // 
            flpAllAvatar.AutoScroll = true;
            flpAllAvatar.Dock = DockStyle.Top;
            flpAllAvatar.FlowDirection = FlowDirection.TopDown;
            flpAllAvatar.Location = new Point(3, 3);
            flpAllAvatar.Name = "flpAllAvatar";
            flpAllAvatar.Size = new Size(481, 360);
            flpAllAvatar.TabIndex = 0;
            flpAllAvatar.WrapContents = false;
            // 
            // groupBoxRight
            // 
            groupBoxRight.Controls.Add(tableLayoutPanel);
            groupBoxRight.Location = new Point(0, 0);
            groupBoxRight.Name = "groupBoxRight";
            groupBoxRight.Size = new Size(290, 419);
            groupBoxRight.TabIndex = 0;
            groupBoxRight.TabStop = false;
            groupBoxRight.Text = "预设设置";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(textBoxProfileName, 1, 0);
            tableLayoutPanel.Controls.Add(label1, 0, 0);
            tableLayoutPanel.Controls.Add(groupBoxAutoEquip, 0, 1);
            tableLayoutPanel.Controls.Add(btnSave, 0, 2);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(3, 19);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.Size = new Size(284, 397);
            tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxProfileName
            // 
            textBoxProfileName.Dock = DockStyle.Fill;
            textBoxProfileName.Location = new Point(63, 3);
            textBoxProfileName.Name = "textBoxProfileName";
            textBoxProfileName.Size = new Size(218, 23);
            textBoxProfileName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 7);
            label1.Margin = new Padding(7);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 1;
            label1.Text = "预设名";
            // 
            // groupBoxAutoEquip
            // 
            tableLayoutPanel.SetColumnSpan(groupBoxAutoEquip, 2);
            groupBoxAutoEquip.Controls.Add(tableLayoutAutoEquip);
            groupBoxAutoEquip.Dock = DockStyle.Fill;
            groupBoxAutoEquip.Location = new Point(10, 39);
            groupBoxAutoEquip.Margin = new Padding(10);
            groupBoxAutoEquip.Name = "groupBoxAutoEquip";
            groupBoxAutoEquip.Size = new Size(264, 319);
            groupBoxAutoEquip.TabIndex = 2;
            groupBoxAutoEquip.TabStop = false;
            groupBoxAutoEquip.Text = "自动装备 [ 可选 ]";
            // 
            // tableLayoutAutoEquip
            // 
            tableLayoutAutoEquip.ColumnCount = 2;
            tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle());
            tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutAutoEquip.Controls.Add(cbEnableAutoEquip, 0, 0);
            tableLayoutAutoEquip.Controls.Add(cbActiveWithScene, 0, 1);
            tableLayoutAutoEquip.Controls.Add(labelSceneId, 0, 2);
            tableLayoutAutoEquip.Controls.Add(tbSceneIds, 1, 2);
            tableLayoutAutoEquip.Controls.Add(cbActiveWithTeam, 0, 3);
            tableLayoutAutoEquip.Controls.Add(labelTeammates, 0, 4);
            tableLayoutAutoEquip.Controls.Add(labelSelectedTeammates, 1, 4);
            tableLayoutAutoEquip.Controls.Add(tbTeammateSearch, 1, 5);
            tableLayoutAutoEquip.Controls.Add(btnUseCurrentTeam, 0, 5);
            tableLayoutAutoEquip.Controls.Add(flpAvatars, 1, 6);
            tableLayoutAutoEquip.Controls.Add(btnClearSelectedTeam, 0, 6);
            tableLayoutAutoEquip.Dock = DockStyle.Fill;
            tableLayoutAutoEquip.Location = new Point(3, 19);
            tableLayoutAutoEquip.Name = "tableLayoutAutoEquip";
            tableLayoutAutoEquip.RowCount = 7;
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
            tableLayoutAutoEquip.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutAutoEquip.Size = new Size(258, 297);
            tableLayoutAutoEquip.TabIndex = 0;
            // 
            // cbEnableAutoEquip
            // 
            cbEnableAutoEquip.AutoSize = true;
            cbEnableAutoEquip.Location = new Point(7, 4);
            cbEnableAutoEquip.Margin = new Padding(7, 4, 7, 4);
            cbEnableAutoEquip.Name = "cbEnableAutoEquip";
            cbEnableAutoEquip.Size = new Size(52, 19);
            cbEnableAutoEquip.TabIndex = 0;
            cbEnableAutoEquip.Text = "启用";
            cbEnableAutoEquip.UseVisualStyleBackColor = true;
            // 
            // cbActiveWithScene
            // 
            cbActiveWithScene.AutoSize = true;
            cbActiveWithScene.Location = new Point(7, 31);
            cbActiveWithScene.Margin = new Padding(7, 4, 5, 4);
            cbActiveWithScene.Name = "cbActiveWithScene";
            cbActiveWithScene.Size = new Size(91, 19);
            cbActiveWithScene.TabIndex = 1;
            cbActiveWithScene.Text = "按场景激活";
            cbActiveWithScene.UseVisualStyleBackColor = true;
            // 
            // labelSceneId
            // 
            labelSceneId.AutoSize = true;
            labelSceneId.Location = new Point(7, 61);
            labelSceneId.Margin = new Padding(7);
            labelSceneId.Name = "labelSceneId";
            labelSceneId.Size = new Size(44, 15);
            labelSceneId.TabIndex = 2;
            labelSceneId.Text = "场景ID";
            // 
            // tbSceneIds
            // 
            tbSceneIds.Dock = DockStyle.Fill;
            tbSceneIds.Location = new Point(106, 57);
            tbSceneIds.Name = "tbSceneIds";
            tbSceneIds.PlaceholderText = "例: 3,1016,1056";
            tbSceneIds.Size = new Size(149, 23);
            tbSceneIds.TabIndex = 3;
            // 
            // cbActiveWithTeam
            // 
            cbActiveWithTeam.AutoSize = true;
            cbActiveWithTeam.Location = new Point(7, 87);
            cbActiveWithTeam.Margin = new Padding(7, 4, 5, 4);
            cbActiveWithTeam.Name = "cbActiveWithTeam";
            cbActiveWithTeam.Size = new Size(91, 19);
            cbActiveWithTeam.TabIndex = 4;
            cbActiveWithTeam.Text = "按队伍激活";
            cbActiveWithTeam.UseVisualStyleBackColor = true;
            // 
            // labelTeammates
            // 
            labelTeammates.AutoSize = true;
            labelTeammates.Location = new Point(7, 117);
            labelTeammates.Margin = new Padding(7);
            labelTeammates.Name = "labelTeammates";
            labelTeammates.Size = new Size(33, 15);
            labelTeammates.TabIndex = 5;
            labelTeammates.Text = "队友";
            // 
            // labelSelectedTeammates
            // 
            labelSelectedTeammates.AutoSize = true;
            labelSelectedTeammates.Location = new Point(103, 117);
            labelSelectedTeammates.Margin = new Padding(0, 7, 7, 7);
            labelSelectedTeammates.Name = "labelSelectedTeammates";
            labelSelectedTeammates.Size = new Size(20, 15);
            labelSelectedTeammates.TabIndex = 6;
            labelSelectedTeammates.Text = "无";
            // 
            // tbTeammateSearch
            // 
            tbTeammateSearch.Dock = DockStyle.Fill;
            tbTeammateSearch.Location = new Point(106, 142);
            tbTeammateSearch.Name = "tbTeammateSearch";
            tbTeammateSearch.PlaceholderText = "人物搜索";
            tbTeammateSearch.Size = new Size(149, 23);
            tbTeammateSearch.TabIndex = 9;
            tbTeammateSearch.TextChanged += tbTeammateSearch_TextChanged;
            // 
            // btnUseCurrentTeam
            // 
            btnUseCurrentTeam.Dock = DockStyle.Fill;
            btnUseCurrentTeam.Location = new Point(3, 142);
            btnUseCurrentTeam.Name = "btnUseCurrentTeam";
            btnUseCurrentTeam.Size = new Size(97, 23);
            btnUseCurrentTeam.TabIndex = 10;
            btnUseCurrentTeam.Text = "使用当前队伍";
            btnUseCurrentTeam.UseVisualStyleBackColor = true;
            btnUseCurrentTeam.Click += btnUseCurrentTeam_Click;
            // 
            // flpAvatars
            // 
            flpAvatars.AutoScroll = true;
            flpAvatars.BackColor = SystemColors.Window;
            flpAvatars.BorderStyle = BorderStyle.FixedSingle;
            flpAvatars.Dock = DockStyle.Fill;
            flpAvatars.FlowDirection = FlowDirection.TopDown;
            flpAvatars.Location = new Point(106, 171);
            flpAvatars.Name = "flpAvatars";
            flpAvatars.Size = new Size(149, 123);
            flpAvatars.TabIndex = 11;
            flpAvatars.WrapContents = false;
            // 
            // btnClearSelectedTeam
            // 
            btnClearSelectedTeam.Dock = DockStyle.Top;
            btnClearSelectedTeam.Location = new Point(3, 171);
            btnClearSelectedTeam.Name = "btnClearSelectedTeam";
            btnClearSelectedTeam.Size = new Size(97, 23);
            btnClearSelectedTeam.TabIndex = 12;
            btnClearSelectedTeam.Text = "清空选择";
            btnClearSelectedTeam.UseVisualStyleBackColor = true;
            btnClearSelectedTeam.Click += btnClearSelectedTeam_Click;
            // 
            // btnSave
            // 
            tableLayoutPanel.SetColumnSpan(btnSave, 2);
            btnSave.Dock = DockStyle.Fill;
            btnSave.Location = new Point(3, 371);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(278, 23);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(49, 17);
            statusLabel.Text = "<状态>";
            // 
            // AddPresetForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip);
            Controls.Add(splitContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddPresetForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "添加预设";
            FormClosed += AddPresetForm_FormClosed;
            Load += AddPresetForm_Load;
            Shown += AddPresetForm_Shown;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            tabCurrentTeam.ResumeLayout(false);
            tabAllAvatar.ResumeLayout(false);
            groupBoxRight.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            groupBoxAutoEquip.ResumeLayout(false);
            tableLayoutAutoEquip.ResumeLayout(false);
            tableLayoutAutoEquip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer;
        private GroupBox groupBoxRight;
        private TabControl tabControl;
        private TabPage tabCurrentTeam;
        private TabPage tabAllAvatar;
        private TableLayoutPanel tableLayoutPanel;
        private TextBox textBoxProfileName;
        private Label label1;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private GroupBox groupBoxAutoEquip;
        private Button btnSave;
        private TableLayoutPanel tableLayoutAutoEquip;
        private CheckBox cbEnableAutoEquip;
        private CheckBox cbActiveWithScene;
        private Label labelSceneId;
        private TextBox tbSceneIds;
        private CheckBox cbActiveWithTeam;
        private Label labelTeammates;
        private Label labelSelectedTeammates;
        private TextBox tbTeammateSearch;
        private Button btnUseCurrentTeam;
        private FlowLayoutPanel flpCurrentTeam;
        private Button btnRefreshTeam;
        private FlowLayoutPanel flpAllAvatar;
        private Button btnRefreshAllAvatar;
        private FlowLayoutPanel flpAvatars;
        private Button btnClearSelectedTeam;
    }
}