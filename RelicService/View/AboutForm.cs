using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using RelicService.Tools;

namespace RelicService.View;

internal class AboutForm : Form
{
	private readonly Network _network;

	private IContainer components;

	private FlowLayoutPanel flowLayoutPanel;

	private Label label1;

	private LinkLabel linkAuthor;

	private LinkLabel linkApi;

	public AboutForm(Network network)
	{
		InitializeComponent();
		_network = network;
		label1.Text = $"Version: {Program.Build}";
	}

	private void linkAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		OpenLink("https://space.bilibili.com/44434084");
	}

	private void linkApi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string swaggerUrl = _network.GetSwaggerUrl();
		OpenLink(swaggerUrl);
	}

	private void OpenLink(string url)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = url,
			UseShellExecute = true
		});
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
        ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutForm));
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
        label1.Size = new Size(60, 20);
        label1.TabIndex = 0;
        label1.Text = "Version:";
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
        linkApi.Size = new Size(84, 15);
        linkApi.TabIndex = 2;
        linkApi.TabStop = true;
        linkApi.Text = "API Document";
        linkApi.LinkClicked += linkApi_LinkClicked;
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(214, 101);
        Controls.Add(flowLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "About";
        flowLayoutPanel.ResumeLayout(false);
        flowLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
