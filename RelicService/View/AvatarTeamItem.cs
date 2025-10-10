using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicService.View
{
    public partial class AvatarTeamItem : UserControl
    {
        private const float WidthToHeightScale = 0.308695652173913f;
        public int ControlIndex { get; set; }
        public Action<AvatarTeamItem, int>? OnDeleteCallback { set; get; }

        public Image AvatarImage1 { set => pbAvatar1.Image = value; }
        public Image AvatarImage2 { set => pbAvatar2.Image = value; }
        public Image AvatarImage3 { set => pbAvatar3.Image = value; }

        public AvatarTeamItem()
        {
            InitializeComponent();

            var size = Size;
            Size = size with { Height = (int)(size.Width * WidthToHeightScale) };
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            OnDeleteCallback?.Invoke(this, ControlIndex);
        }
    }
}
