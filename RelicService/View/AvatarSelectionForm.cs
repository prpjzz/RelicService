using RelicService.Tools;
using RelicService.Service;
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
    internal partial class AvatarSelectionForm : Form
    {
        private AvatarService _avatarService;
        private StatusService _statusService;
        private ResourceManager _resourceManager;

        private HashSet<uint> _avatarImageRef = new();
        private PictureBox? _lastSelectedPictureBox = null;

        public bool IsMultiSelect { get; set; } = false;
        public uint MultiSelectLimit { get; set; } = 1;
        public List<ulong> SelectedAvatarGuids { get; private set; } = new();

        public ulong SelectedAvatarGuid
        {
            get
            {
                if (_lastSelectedPictureBox == null)
                    return 0;

                return (ulong)_lastSelectedPictureBox.Tag;
            }
        }

        public AvatarSelectionForm() { }

        public AvatarSelectionForm(
            AvatarService avatarService,
            StatusService statusService,
            ResourceManager resourceManager
            )
        {
            InitializeComponent();
            _avatarService = avatarService;
            _statusService = statusService;
            _resourceManager = resourceManager;
        }

        private async void AvatarSelectionForm_Load(object sender, EventArgs e)
        {
            btnConfirm.Enabled = false;
            await PopulateGrid();
        }


        private void AvatarSelectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _avatarImageRef
                .ToList()
                .ForEach(_resourceManager.FreeAvatarImage);
        }

        private void AvatarPictureBox_Click(object? sender, EventArgs e)
        {
            if (!(sender is PictureBox pictureBox))
                return;

            if (!IsMultiSelect)
            {
                if (_lastSelectedPictureBox != null)
                    _lastSelectedPictureBox.BackColor = Color.Transparent;

                pictureBox.BackColor = SystemColors.ControlDark;
                _lastSelectedPictureBox = pictureBox;
            }
            else
            {
                var wasSelected = pictureBox.BackColor == SystemColors.ControlDark;
                if (wasSelected)
                {
                    pictureBox.BackColor = Color.Transparent;
                    SelectedAvatarGuids.Remove((ulong)pictureBox.Tag);
                }
                else
                {
                    var limitReached = SelectedAvatarGuids.Count >= MultiSelectLimit;
                    if (limitReached)
                        return;

                    pictureBox.BackColor = SystemColors.ControlDark;
                    SelectedAvatarGuids.Add((ulong)pictureBox.Tag);
                }

                btnConfirm.Enabled = SelectedAvatarGuids.Count > 0;
            }

            btnConfirm.Enabled = true;
        }

        private void AvatarPictureBox_DoubleClick(object? sender, EventArgs e)
        {
            if (_lastSelectedPictureBox == null)
                return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private async Task PopulateGrid()
        {
            var userAvatars = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
            if (userAvatars == null)
                return;

            userAvatars = userAvatars.OrderBy(x => x.AvatarId).ToList();

            foreach (var userAvatar in userAvatars)
            {
                var avatarImage = await _resourceManager.GetAvatarImage(userAvatar.AvatarId);
                if (avatarImage == null)
                    continue;

                avatarImage = Utils.ResizeImage(avatarImage, 80, 80);
                _avatarImageRef.Add(userAvatar.AvatarId);

                var pictureBox = new PictureBox
                {
                    Image = avatarImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = userAvatar.Guid,
                    Size = new Size(80, 80),
                    Margin = new Padding(8, 4, 3, 3),
                    Cursor = Cursors.Hand
                };

                pictureBox.Click += AvatarPictureBox_Click;
                pictureBox.DoubleClick += AvatarPictureBox_DoubleClick;

                tlpAvatars.Controls.Add(pictureBox);
            }

        }

    }
}
