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
    public partial class AvatarRelicInfo : UserControl
    {
        private bool _isSelected = false;
        private ulong _avatarGuid;
        private Action<ulong>? _onAnyItemClicked;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                tableLayoutPanel.BackColor = _isSelected ? SystemColors.ControlDark : SystemColors.Menu;
            }
        }

        public Image AvatarImage { set => pbAvatar.Image = value; }
        public Image FlowerImage { set => pbFlower.Image = value; }
        public Image FeatherImage { set => pbFeather.Image = value; }
        public Image SandImage { set => pbSand.Image = value; }
        public Image GobletImage { set => pbGoblet.Image = value; }
        public Image HeadImage { set => pbHead.Image = value; }

        public string FlowerTooltip { set => toolTip.SetToolTip(pbFlower, value); }
        public string FeatherTooltip { set => toolTip.SetToolTip(pbFeather, value); }
        public string SandTooltip { set => toolTip.SetToolTip(pbSand, value); }
        public string GobletTooltip { set => toolTip.SetToolTip(pbGoblet, value); }
        public string HeadTooltip { set => toolTip.SetToolTip(pbHead, value); }

        public Action<ulong> OnClickCallback { set => _onAnyItemClicked = value; }

        public AvatarRelicInfo(ulong avatarGuid)
        {
            InitializeComponent();
            _avatarGuid = avatarGuid;

            tableLayoutPanel.Controls
                .OfType<PictureBox>()
                .ToList()
                .ForEach(pb =>
                {
                    pb.Image = null;
                    pb.Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);
                });

            tableLayoutPanel.Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);
            Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);
        }


    }
}
