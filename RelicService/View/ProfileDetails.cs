using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Service;
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
    internal partial class ProfileDetails : UserControl
    {
        public static readonly float WidthToHeightScale = 0.1942857142857143f;

        private EquipService _equipService;
        private AvatarService _avatarService;
        private GameMessageService _gameMessageService;

        public string ProfileName
        {
            get => groupBox.Text;
            set => groupBox.Text = value;
        }

        public Image FlowerImage { set => pbFlower.Image = value; }
        public Image FeatherImage { set => pbFeather.Image = value; }
        public Image SandImage { set => pbSand.Image = value; }
        public Image GobletImage { set => pbGoblet.Image = value; }
        public Image HeadImage { set => pbHead.Image = value; }

        public string FlowerLabel { set => labelFlower.Text = value; }
        public string FeatherLabel { set => labelFeather.Text = value; }
        public string SandLabel { set => labelSand.Text = value; }
        public string GobletLabel { set => labelGoblet.Text = value; }
        public string HeadLabel { set => labelHead.Text = value; }

        public string ConflicLabel { set => labelConflict.Text = value; }

        public DbRelicProfile? RelicProfile { get; set; }

        private readonly object _lock = new();

        public ProfileDetails() { }
        public ProfileDetails(
            EquipService equipService,
            AvatarService avatarService,
            GameMessageService gameMessageService
            )
        {
            InitializeComponent();
            _equipService = equipService;
            _avatarService = avatarService;
            _gameMessageService = gameMessageService;

            FlowerImage = null;
            FeatherImage = null;
            SandImage = null;
            GobletImage = null;
            HeadImage = null;

            FlowerLabel = string.Empty;
            FeatherLabel = string.Empty;
            SandLabel = string.Empty;
            GobletLabel = string.Empty;
            HeadLabel = string.Empty;
        }


        private async void btnQuickEquip_Click(object sender, EventArgs e)
        {
            if (RelicProfile == null)
                return;

            var avatarName = RelicProfile.UserAvatar.Avatar.Name;
            var avatarGuid = RelicProfile.AvatarGuid;

            await EquipToAvatar(avatarGuid);

            _gameMessageService.EnqueueMessage($"已为 [{avatarName}] 装备 [{RelicProfile.ProfileName}] 预设");
        }

        private async void btnEquipTo_Click(object sender, EventArgs e)
        {
            if (RelicProfile == null)
                return;

            var form = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
            if (form.ShowDialog() == DialogResult.Cancel)
                return;

            var selectedAvatarGuid = form.SelectedAvatarGuid;
            var userAvatar = await _avatarService.GetUserAvatarByGuid(selectedAvatarGuid);
            if (userAvatar == null)
                return;

            var profileAvatarName = RelicProfile.UserAvatar.Avatar.Name;
            var avatarName = userAvatar.Avatar.Name;
            await EquipToAvatar(selectedAvatarGuid);

            _gameMessageService.EnqueueMessage($"已为 [{avatarName}] 装备 [{profileAvatarName}->{RelicProfile.ProfileName}] 预设");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<ProfileEditForm>();
            form.RelicProfile = RelicProfile;
            form.ShowDialog();
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_lock, ref lockTaken);
                if (!lockTaken)
                    return;

                var pb = sender as PictureBox;
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

        private async Task EquipToAvatar(ulong avatarGuid)
        {
            if (RelicProfile?.RelicItems == null)
                return;

            foreach (var relic in RelicProfile.RelicItems)
            {
                var relicGuid = relic.Guid;
                await _equipService.WearEquip(avatarGuid, relicGuid);
            }
        }

    }
}
