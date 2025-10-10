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
    public partial class AvatarProfileItem : UserControl
    {
        private static readonly float WidthToHeightScale = 0.2777777777777778f;

        private bool _isSelected = false;
        private ulong _avatarGuid;
        private Action<ulong>? _onAnyItemClicked;

        private readonly object _lock = new();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                tableLayoutPanel.BackColor = _isSelected ? SystemColors.ControlDark : SystemColors.Window;
            }
        }

        public Image AvatarImage { set => pbAvatar.Image = value; }
        public string AvatarName { get => labelAvatarName.Text; set => labelAvatarName.Text = value; }
        public Action<ulong> OnClickCallback { set => _onAnyItemClicked = value; }

        public AvatarProfileItem() { }

        public AvatarProfileItem(ulong avatarGuid)
        {
            InitializeComponent();
            _avatarGuid = avatarGuid;

            tableLayoutPanel.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(c =>
                {
                    c.Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);
                });

            tableLayoutPanel.Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);
            Click += (sender, e) => _onAnyItemClicked?.Invoke(_avatarGuid);

            pbAvatar.Image = null;

            var size = Size;
            Size = size with { Height = (int)(size.Width * WidthToHeightScale) };
        }

        private void pbAvatar_Resize(object sender, EventArgs e)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_lock, ref lockTaken);
                if (!lockTaken)
                    return;

                var pb = (PictureBox)sender;
                pb.Size = pb.Size with { Height = pb.Size.Width };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_lock);
            }

        }

    }
}
