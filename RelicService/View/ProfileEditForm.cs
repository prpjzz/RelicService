using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Tools;
using RelicService.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicService.View
{
    internal partial class ProfileEditForm : Form
    {
        private readonly AvatarService _avatarService;
        private readonly ResourceManager _resourceManager;
        private readonly EventManager _eventManager;
        private readonly SqliteContext _dbContext;

        private readonly Dictionary<uint, Image> _avatarImageMap = new();
        private readonly HashSet<uint> _avatarImageRef = new();

        public DbRelicProfile? RelicProfile { get; set; }

        public ProfileEditForm(
            AvatarService avatarService,
            ResourceManager resourceManager,
            EventManager eventManager,
            SqliteContext dbContext
            )
        {
            InitializeComponent();
            _avatarService = avatarService;
            _resourceManager = resourceManager;
            _eventManager = eventManager;
            _dbContext = dbContext;
        }

        private async void ProfileEditForm_Load(object sender, EventArgs e)
        {
            SetupBindings();

            if (RelicProfile == null)
                return;

            Text = $@"编辑配置 - {RelicProfile.ProfileName}";

            if (RelicProfile.WithScene.Count > 0)
            {
                tbSceneIds.Text = RelicProfile.WithScene
                    .Select(s => s.ToString())
                    .Aggregate((a, b) => $"{a},{b}");
            }

            // TODO: Load team context
            await PopulateTeamList();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await _dbContext.SaveChangesAsync();
            _eventManager.FireEventAsync(Data.Event.EventId.EvtProfileRefresh, null);
            Close();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(@"确定删除？", @"删除配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            if (RelicProfile == null)
                return;

            _dbContext.RelicProfiles.Remove(RelicProfile);
            await _dbContext.SaveChangesAsync();
            _eventManager.FireEventAsync(Data.Event.EventId.EvtProfileRefresh, null);
            Close();
        }

        private async void btnAddTeam_Click(object sender, EventArgs e)
        {
            if (RelicProfile == null)
                return;

            var form = Program.ServiceProvider.GetRequiredService<AvatarSelectionForm>();
            form.IsMultiSelect = true;
            form.MultiSelectLimit = 3;
            if (form.ShowDialog() == DialogResult.Cancel)
                return;

            var guids = form.SelectedAvatarGuids;
            var userAvatars = (await Task.WhenAll(guids
                .Select(async g => await _avatarService.GetUserAvatarByGuid(g))))
                .Where(u => u != null)
                .ToList();

            var avatarIds = userAvatars
                .Select(ua => ua.Avatar.AvatarId)
                .Order()
                .ToList();

            // exact unordered match content
            var hasMatch = RelicProfile.TeamContexts
                .Any(tc => tc.AvatarIds.Count == avatarIds.Count && tc.AvatarIds.All(avatarIds.Contains));
            if (hasMatch)
            {
                MessageBox.Show(@"队伍已存在", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var teamContext = new DbRelicProfileTeamContext
            {
                AvatarIds = avatarIds,
                Profile = RelicProfile
            };

            RelicProfile.TeamContexts.Add(teamContext);

            await PopulateTeamList();
        }

        private void ProfileEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _avatarImageRef
                .ToList()
                .ForEach(_resourceManager.FreeAvatarImage);

        }

        private async void AvatarTeamItem_OnDeleteClick(AvatarTeamItem panel, int controlIndex)
        {

            var teamContext = RelicProfile?.TeamContexts.ElementAtOrDefault(controlIndex);
            if (teamContext == null)
                return;

            RelicProfile?.TeamContexts.Remove(teamContext);
            flpTeams.Controls.Remove(panel);
            panel.Dispose();

            await PopulateTeamList();
        }

        private void SetupBindings()
        {
            tbProfileName.DataBindings.Add("Text", RelicProfile, "ProfileName");
            cbAutoEquip.DataBindings.Add("Checked", RelicProfile, "AutoEquip");

            var binding = new Binding("Text", RelicProfile, "WithScene");
            binding.Format += (s, e) =>
            {
                if (e.Value is List<uint> sceneIds)
                    e.Value = string.Join(",", sceneIds);
            };

            // Parse: Convert comma-separated string back to List<uint>, silently ignoring invalid inputs
            binding.Parse += (s, e) =>
            {
                if (e.Value is string text)
                {
                    text = text.Replace(" ", "");   // Remove spaces
                    var parsedList = text.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                         .Select(str =>
                                         {
                                             uint result;
                                             bool success = uint.TryParse(str, out result);
                                             return (success, result);
                                         })
                                         .Where(p => p.success)   // Only take successfully parsed values
                                         .Select(p => p.result)
                                         .ToList();

                    e.Value = parsedList;
                }
            };
            tbSceneIds.DataBindings.Add(binding);

            labelSceneId.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
            tbSceneIds.DataBindings.Add("Enabled", cbAutoEquip, "Checked");

            labelTeam.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
            flpTeams.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
            btnAddTeam.DataBindings.Add("Enabled", cbAutoEquip, "Checked");
        }

        private async Task PopulateTeamList()
        {
            flpTeams.Controls
                .OfType<AvatarTeamItem>()
                .ToList()
                .ForEach(c => c.Dispose());

            flpTeams.Controls.Clear();

            if (RelicProfile == null)
                return;

            foreach (var team in RelicProfile.TeamContexts)
            {
                var panel = new AvatarTeamItem();
                panel.ControlIndex = flpTeams.Controls.Count;
                panel.OnDeleteCallback = AvatarTeamItem_OnDeleteClick;

                List<Image> images = new();
                foreach (var avatarId in team.AvatarIds)
                {

                    if (!_avatarImageMap.TryGetValue(avatarId, out var avatarImage))
                    {
                        var avatarMeta = await _avatarService.GetAvatarMetadata(avatarId);
                        if (avatarMeta == null)
                            continue;

                        avatarImage = await _resourceManager.GetAvatarImage(avatarMeta.AvatarId);
                        if (avatarImage == null)
                            continue;

                        avatarImage = Utils.ResizeImage(avatarImage, 80, 80);

                        _avatarImageRef.Add(avatarMeta.AvatarId);
                        _avatarImageMap.Add(avatarId, avatarImage);
                    }

                    images.Add(avatarImage);
                }

                panel.AvatarImage1 = images.ElementAtOrDefault(0);
                panel.AvatarImage2 = images.ElementAtOrDefault(1);
                panel.AvatarImage3 = images.ElementAtOrDefault(2);

                flpTeams.Controls.Add(panel);
            }

        }


    }
}
