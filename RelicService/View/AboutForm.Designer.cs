namespace RelicService.View
{
    partial class AboutForm
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
            flowLayoutPanel = new FlowLayoutPanel();
            label1 = new Label();
            linkAuthor = new LinkLabel();
            linkApi = new LinkLabel();
            flowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Controls.Add(label1);
            flowLayoutPanel.Controls.Add(linkAuthor);
            flowLayoutPanel.Controls.Add(linkApi);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(10);
            flowLayoutPanel.Size = new Size(214, 101);
            flowLayoutPanel.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(13, 10);
            label1.Name = "label1";
            label1.Size = new Size(44, 20);
            label1.TabIndex = 0;
            label1.Text = "版本:";
            // 
            // linkAuthor
            // 
            linkAuthor.AutoSize = true;
            linkAuthor.LinkArea = new LinkArea(4, 8);
            linkAuthor.Location = new Point(15, 30);
            linkAuthor.Margin = new Padding(5, 0, 3, 0);
            linkAuthor.Name = "linkAuthor";
            linkAuthor.Size = new Size(52, 21);
            linkAuthor.TabIndex = 1;
            linkAuthor.TabStop = true;
            linkAuthor.Text = "by: Ex_M";
            linkAuthor.UseCompatibleTextRendering = true;
            linkAuthor.LinkClicked += linkAuthor_LinkClicked;
            // 
            // linkApi
            // 
            linkApi.AutoSize = true;
            linkApi.Location = new Point(15, 61);
            linkApi.Margin = new Padding(5, 10, 3, 0);
            linkApi.Name = "linkApi";
            linkApi.Size = new Size(51, 15);
            linkApi.TabIndex = 2;
            linkApi.TabStop = true;
            linkApi.Text = "API文档";
            linkApi.LinkClicked += linkApi_LinkClicked;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(214, 101);
            Controls.Add(flowLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "关于";
            flowLayoutPanel.ResumeLayout(false);
            flowLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel;
        private Label label1;
        private LinkLabel linkAuthor;
        private LinkLabel linkApi;
    }
}