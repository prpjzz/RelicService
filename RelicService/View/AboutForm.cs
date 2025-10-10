using RelicService.Tools;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicService.View
{
    internal partial class AboutForm : Form
    {
        private readonly Network _network;

        public AboutForm(Network network)
        {
            InitializeComponent();

            _network = network;
            label1.Text = $@"版本: {Program.Build}";
        }

        private void linkAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://space.bilibili.com/44434084");
        }

        private void linkApi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = _network.GetSwaggerUrl();
            OpenLink(link);
        }

        private void OpenLink(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

    }
}
