// Decompiled with JetBrains decompiler
// Type: RelicService.View.AboutForm
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Tools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
  internal class AboutForm : Form
  {
    private readonly Network _network;
    private 
    #nullable disable
    IContainer components;
    private FlowLayoutPanel flowLayoutPanel;
    private Label label1;
    private LinkLabel linkAuthor;
    private LinkLabel linkApi;

    public AboutForm(
    #nullable enable
    Network network)
    {
      this.InitializeComponent();
      this._network = network;
      Label label1 = this.label1;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
      interpolatedStringHandler.AppendLiteral("版本: ");
      interpolatedStringHandler.AppendFormatted<int>(Program.Build);
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      label1.Text = stringAndClear;
    }

    private void linkAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.OpenLink("https://space.bilibili.com/44434084");
    }

    private void linkApi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.OpenLink(this._network.GetSwaggerUrl());
    }

    private void OpenLink(string url)
    {
      Process.Start(new ProcessStartInfo()
      {
        FileName = url,
        UseShellExecute = true
      });
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        ((IDisposable) this.components).Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.flowLayoutPanel = new FlowLayoutPanel();
      this.label1 = new Label();
      this.linkAuthor = new LinkLabel();
      this.linkApi = new LinkLabel();
      this.flowLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      this.flowLayoutPanel.Controls.Add((Control) this.label1);
      this.flowLayoutPanel.Controls.Add((Control) this.linkAuthor);
      this.flowLayoutPanel.Controls.Add((Control) this.linkApi);
      this.flowLayoutPanel.Dock = DockStyle.Fill;
      this.flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel.Location = new Point(0, 0);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Padding = new Padding(10);
      this.flowLayoutPanel.Size = new Size(214, 101);
      this.flowLayoutPanel.TabIndex = 0;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 11.25f, (FontStyle) 0, (GraphicsUnit) 3, (byte) 0);
      this.label1.Location = new Point(13, 10);
      this.label1.Name = "label1";
      this.label1.Size = new Size(44, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "版本:";
      this.linkAuthor.AutoSize = true;
      this.linkAuthor.LinkArea = new LinkArea(4, 8);
      this.linkAuthor.Location = new Point(15, 30);
      this.linkAuthor.Margin = new Padding(5, 0, 3, 0);
      this.linkAuthor.Name = "linkAuthor";
      this.linkAuthor.Size = new Size(52, 21);
      this.linkAuthor.TabIndex = 1;
      this.linkAuthor.TabStop = true;
      this.linkAuthor.Text = "by: Ex_M";
      this.linkAuthor.UseCompatibleTextRendering = true;
      this.linkAuthor.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkAuthor_LinkClicked);
      this.linkApi.AutoSize = true;
      this.linkApi.Location = new Point(15, 61);
      this.linkApi.Margin = new Padding(5, 10, 3, 0);
      this.linkApi.Name = "linkApi";
      this.linkApi.Size = new Size(51, 15);
      this.linkApi.TabIndex = 2;
      this.linkApi.TabStop = true;
      this.linkApi.Text = "API文档";
      this.linkApi.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkApi_LinkClicked);
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(214, 101);
      this.Controls.Add((Control) this.flowLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AboutForm);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "关于";
      this.flowLayoutPanel.ResumeLayout(false);
      this.flowLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
