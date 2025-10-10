using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Event;
using RelicService.Tools;
using RelicService.Service;
using RelicService.View;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RelicService.Data;
using System.Security.Policy;


namespace RelicService.View
{
    internal partial class MainForm : Form
    {
        private static readonly Size ProfileDetailSize = new Size(875, 170);
        private static readonly Size ProfileDetailSizeWithScroll = new Size(860, 170);

        private readonly EventManager _eventManager;
        private readonly ResourceManager _resourceManager;
        private readonly StatusService _statusService;
        private readonly AutoEquipService _autoEquipService;
        private readonly GameMessageService _gameMessageService;
        private readonly Network _network;
        private ApiService _apiService;
        private readonly SqliteContext _dbContext;

        private bool _isOnline = false;
        private bool _isOffline = true;
        private uint _lastSceneId = 0;
        private ulong _lastSelectedAvatarGuid = 0;

        private readonly Dictionary<ulong, AvatarProfileItem> _avatarProfileItemMap = new();
        private readonly Dictionary<uint, Image> _avatarImageCache = new();
        private readonly Dictionary<uint, Image> _relicImageCache = new();
        private readonly HashSet<uint> _relicImageRef = new();

        public MainForm() { }

        public MainForm(
            EventManager eventManager,
            ResourceManager resourceManager,
            StatusService statusService,
            AutoEquipService autoEquipService,
            GameMessageService gameMessageService,
            Network network,
            ApiService apiService,
            SqliteContext dbContext
            )
        {
            _eventManager = eventManager;
            _resourceManager = resourceManager;
            _statusService = statusService;
            _autoEquipService = autoEquipService;
            _gameMessageService = gameMessageService;
            _network = network;
            _apiService = apiService;
            _dbContext = dbContext;

            InitializeComponent();

            _eventManager.OnServiceStatusChanged += OnServiceStatusChanged;
            _eventManager.OnUidChanged += OnUidChanged;
            _eventManager.OnSceneIdChanged += OnSceneChanged;
            _eventManager.OnProfileRefresh += OnProfileRefresh;
            _eventManager.OnProfileConflict += OnProfileConflict;
        }

        #region Form Events

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupBindings();

            labelConflict.Text = string.Empty;
            btnConfirmConflict.Visible = false;

            using Graphics g = CreateGraphics();
            Program.DpiScaleFactor = g.DpiX / 96f;

            _ = Task.Run(CheckVersion);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _eventManager.FireEventAsync(EventId.EvtShutdown);
        }

        private async void btnAddPreset_Click(object sender, EventArgs e)
        {
            using var form = Program.ServiceProvider.GetRequiredService<AddPresetForm>();
            form.ShowDialog();

            await PopulateAvatarList();
            await ChangeRelicDetailDisplay();
        }

        //private void flpProfileDetails_ControlAdded(object sender, ControlEventArgs e)
        //{
        //    CheckProfileDetailSize();
        //}

        //private void flpProfileDetails_ControlRemoved(object sender, ControlEventArgs e)
        //{
        //    CheckProfileDetailSize();
        //}

        private void btnConfirmConflict_Click(object sender, EventArgs e)
        {
            labelConflict.Text = string.Empty;
            btnConfirmConflict.Visible = false;
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<AboutForm>();
            form.ShowDialog();
        }

        private void textBoxAvatarFilter_TextChanged(object sender, EventArgs e)
        {
            var filter = textBoxAvatarFilter.Text;
            foreach (var control in flpAvatars.Controls) {
                if (control is AvatarProfileItem item)
                    item.Visible = item.AvatarName.Contains(filter) || string.IsNullOrWhiteSpace(filter);
            }
        }

        private async void tsOpenCrafting_Click(object sender, EventArgs e)
        {
            try
            {
                await _network.OpenCrafting();
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion

        private async Task CheckVersion()
        {
            try {
                var result = await _network.GetVersionInfo();
                if (result == null)
                    return;

                var versionInfo = JsonConvert.DeserializeObject<VersionInfo>(result);
                if (versionInfo == null)
                    return;

                if (versionInfo.Build <= Program.Build)
                    return;

                Invoke(() => {
                    var result = MessageBox.Show(@"有新版本可用，是否前往下载？", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes) {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "https://space.bilibili.com/44434084/dynamic",
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                });
            }
            catch (Exception) {
                // ignored
            }
        }

        private void SetupBindings()
        {
            btnAddPreset.Enabled = _isOnline;
            cbAutoEquip.DataBindings.Add("Checked", _autoEquipService, nameof(_autoEquipService.Enabled), false, DataSourceUpdateMode.OnPropertyChanged);
            cbEnableGameMessage.DataBindings.Add("Checked", _gameMessageService, nameof(_gameMessageService.Enabled), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private async Task PopulateAvatarList()
        {
            _avatarProfileItemMap.Clear();
            _lastSelectedAvatarGuid = 0;
            ClearAvatarList();

            await Task.Run(async () => {
                while (_statusService.CurrentUid == 0)
                    await Task.Delay(100);
            });

            var profiles = await _dbContext.RelicProfiles
                .Include(p => p.UserAvatar)
                .ThenInclude(ua => ua.Avatar)
                .Where(p => p.UserAvatar.UserUid == _statusService.CurrentUid)
                .ToListAsync();

            var userAvatars = profiles
                .Select(p => p.UserAvatar)
                .Distinct()
                .OrderBy(a => a.AvatarId)
                .ToList();

            foreach (var userAvatar in userAvatars) {
                if (!_avatarImageCache.TryGetValue(userAvatar.AvatarId, out var avatarImage)) {
                    avatarImage = await _resourceManager.GetAvatarImage(userAvatar.AvatarId);
                    if (avatarImage == null)
                        continue;

                    avatarImage = Utils.ResizeImage(avatarImage, 65, 65);
                    _avatarImageCache.Add(userAvatar.AvatarId, avatarImage);
                }

                var panel = new AvatarProfileItem(userAvatar.Guid);
                panel.AvatarImage = avatarImage;
                panel.AvatarName = userAvatar.Avatar.Name;
                panel.OnClickCallback = OnAvatarProfileItemClicked;

                _avatarProfileItemMap.TryAdd(userAvatar.Guid, panel);
                flpAvatars.Controls.Add(panel);
            }

        }

        private async void OnAvatarProfileItemClicked(ulong avatarGuid)
        {
            if (_lastSelectedAvatarGuid != 0) {
                var control = _avatarProfileItemMap[_lastSelectedAvatarGuid];
                control.IsSelected = false;
            }

            var selectedControl = _avatarProfileItemMap[avatarGuid];
            selectedControl.IsSelected = true;

            _lastSelectedAvatarGuid = avatarGuid;
            await ChangeRelicDetailDisplay();
        }

        private async Task ChangeRelicDetailDisplay()
        {
            // clear all controls
            flpProfileDetails.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(c => c.Dispose());
            flpProfileDetails.Controls.Clear();

            // free all relic images
            _relicImageRef
                .ToList()
                .ForEach(_resourceManager.FreeRelicImage);
            _relicImageRef.Clear();

            var profiles = await _dbContext.RelicProfiles
                .Include(p => p.UserAvatar)
                    .ThenInclude(ua => ua.Avatar)
                .Include(p => p.RelicItems)
                    .ThenInclude(r => r.Relic)
                .Include(p => p.RelicItems)
                    .ThenInclude(r => r.Affixes)
                .Include(p => p.TeamContexts)
                .Where(p => p.UserAvatar.Guid == _lastSelectedAvatarGuid)
                .ToListAsync();

            if (profiles.Count == 0)
                return;

            var avatar = profiles.First().UserAvatar;
            groupBoxSelectedAvatar.Text = avatar.Avatar.Name;

            foreach (var profile in profiles) {
                var panel = await CreateProfileDetailPanel(profile);
                flpProfileDetails.Controls.Add(panel);
            }

            CheckProfileDetailSize();
        }

        private async Task<ProfileDetails> CreateProfileDetailPanel(DbRelicProfile profile)
        {
            var panel = Program.ServiceProvider.GetRequiredService<ProfileDetails>();
            panel.ProfileName = profile.ProfileName;
            panel.RelicProfile = profile;
            panel.Dock = DockStyle.Top;

            var parentHeight = flpProfileDetails.ClientSize.Height;
            var parentWidth = flpProfileDetails.Width - flpProfileDetails.Margin.Horizontal;

            panel.MinimumSize = new Size(parentWidth, (int)(parentWidth * ProfileDetails.WidthToHeightScale));
            //panel.Size = panel.MinimumSize;


            foreach (var relic in profile.RelicItems) {
                if (!_relicImageCache.TryGetValue(relic.ItemId, out var relicImage)) {
                    relicImage = await _resourceManager.GetRelicImage(relic.ItemId);
                    if (relicImage == null)
                        continue;

                    relicImage = Utils.ResizeImage(relicImage, 80, 80);
                    _relicImageCache.Add(relic.ItemId, relicImage);
                    _relicImageRef.Add(relic.ItemId);
                }

                var sb = new StringBuilder();
                sb.AppendLine(Utils.FormatFightPropShort(relic.MainPropType, relic.MainPropValue));
                sb.AppendLine();

                foreach (var prop in relic.Affixes) {
                    var propStr = Utils.FormatFightPropShort(prop.PropType, prop.PropValue);
                    sb.AppendLine(propStr);
                }

                switch (relic.EquipType) {
                    case EquipType.EQUIP_BRACER:
                        panel.FlowerImage = relicImage;
                        panel.FlowerLabel = sb.ToString();
                        break;
                    case EquipType.EQUIP_NECKLACE:
                        panel.FeatherImage = relicImage;
                        panel.FeatherLabel = sb.ToString();
                        break;
                    case EquipType.EQUIP_SHOES:
                        panel.SandImage = relicImage;
                        panel.SandLabel = sb.ToString();
                        break;
                    case EquipType.EQUIP_RING:
                        panel.GobletImage = relicImage;
                        panel.GobletLabel = sb.ToString();
                        break;
                    case EquipType.EQUIP_DRESS:
                        panel.HeadImage = relicImage;
                        panel.HeadLabel = sb.ToString();
                        break;
                }

            }

            panel.ConflicLabel = "";
            panel.ConflicLabel = await CheckProfileConflict(profile);

            return panel;
        }

        private async Task<string> CheckProfileConflict(DbRelicProfile profile)
        {
            if (profile.WithScene.Count == 0 && profile.TeamContexts.Count == 0)
                return string.Empty;

            var otherProfiles = await _dbContext.RelicProfiles
                .Include(p => p.RelicItems)
                    .ThenInclude(r => r.Relic)
                .Include(p => p.UserAvatar)
                    .ThenInclude(ua => ua.Avatar)
                .Include(p => p.TeamContexts)
                .Where(p => p.Id != profile.Id && p.UserAvatar.UserUid == profile.UserAvatar.UserUid)
                .ToListAsync();

            ConcurrentBag<DbRelicProfile> teamConflicts = new();
            ConcurrentBag<DbRelicProfile> sceneConflicts = new();

            await Task.Run(() => {
                Parallel.ForEach(otherProfiles, otherProfiles_ => {
                    if (ProfileHasTeamConflict(profile, otherProfiles_))
                        teamConflicts.Add(otherProfiles_);

                    if (ProfileHasSceneConflict(profile, otherProfiles_))
                        sceneConflicts.Add(otherProfiles_);
                });
            });

            if (teamConflicts.Count == 0 && sceneConflicts.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("冲突:");

            if (sceneConflicts.Count > 0) {
                sb.Append("场景(");
                sb.Append(sceneConflicts
                    .Select(p => p.ProfileName)
                    .Aggregate((a, b) => $"{a},{b}"));
                sb.Append(") ");
            }

            if (teamConflicts.Count > 0) {
                sb.Append("队伍(");
                sb.Append(teamConflicts
                    .Select(p => p.ProfileName)
                    .Aggregate((a, b) => $"{a},{b}"));
                sb.Append(") ");
            }

            return sb.ToString();
        }

        private bool ProfileHasTeamConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
        {
            var thisRelics = profile.RelicItems
                .Select(r => r.Guid);
            var otherRelics = otherProfile.RelicItems
                .Select(r => r.Guid);

            var relicHasOverlap = thisRelics.Intersect(otherRelics).Any();
            if (relicHasOverlap) {
                // same relic has been found in both profiles
                // we now need to check if the activation teams have overlap
                foreach (var thisTeam in profile.TeamContexts) {
                    foreach (var otherTeam in otherProfile.TeamContexts) {
                        if (thisTeam.AvatarIds.Intersect(otherTeam.AvatarIds).Any())
                            return true;
                    }
                }
            }

            return false;
        }

        private bool ProfileHasSceneConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
        {
            // we only need to check for profiles with current avatar
            if (profile.UserAvatar.Guid != otherProfile.UserAvatar.Guid)
                return false;

            return profile.WithScene.Intersect(otherProfile.WithScene).Any();
        }

        private void CheckProfileDetailSize()
        {
            var controls = flpProfileDetails.Controls
                .OfType<ProfileDetails>()
                .ToList();

            var scrollBar = flpProfileDetails.Controls
                .OfType<ScrollableControl>()
                .FirstOrDefault();

            var minWidth = flpProfileDetails.ClientSize.Width - flpProfileDetails.Margin.Horizontal;
            if (flpProfileDetails.VerticalScroll.Visible)
                minWidth = flpProfileDetails.Width - SystemInformation.VerticalScrollBarWidth - flpProfileDetails.Margin.Horizontal;

            controls.ForEach(x => x.MinimumSize = new Size(minWidth, (int)(minWidth * ProfileDetails.WidthToHeightScale)));
            //controls.ForEach(x => x.Size = x.MinimumSize);
        }

        private void OnServiceStatusChanged(object? sender, bool isOnline)
        {
            if (InvokeRequired) {
                Invoke(() => OnServiceStatusChanged(sender, isOnline));
                return;
            }

            _isOnline = isOnline;
            _isOffline = !isOnline;

            btnAddPreset.Enabled = _isOnline;
            serverStatus.Text = _isOnline ? "服务在线" : "服务离线";
            serverStatus.Image = _isOnline ? Properties.Resources.dot_green : Properties.Resources.dot_red;

            if (_isOffline) {
                _lastSelectedAvatarGuid = 0;
                ClearAvatarList();
                ClearProfileDetails();

                labelUid.Text = @"UID: ";
                labelScene.Text = @"场景: ";
            }
        }

        private void UpdateUidText(uint uid)
        {

            // replace with '*' except for first 2 and last 2 characters
            var uidText = uid.ToString();
            if (uidText.Length > 3) {
                var sb = new StringBuilder(uidText);
                for (int i = 2; i < uidText.Length - 2; i++)
                    sb[i] = '*';

                uidText = sb.ToString();
            }
            labelUid.Text = $@"UID: {uidText}";
        }

        private async void OnUidChanged(object? sender, uint uid)
        {
            if (InvokeRequired) {
                Invoke(() => OnUidChanged(sender, uid));
                return;
            }

            UpdateUidText(uid);

            if (uid == 0) {
                _avatarProfileItemMap.Clear();
                ClearProfileDetails();
                return;
            }

            _lastSelectedAvatarGuid = 0;
            await PopulateAvatarList();

            ClearProfileDetails();
        }

        private void OnSceneChanged(object? sender, uint sceneId)
        {
            if (InvokeRequired) {
                Invoke(() => OnSceneChanged(sender, sceneId));
                return;
            }

            labelScene.Text = $@"场景: {sceneId.ToString()}";

            if (sceneId == 0) {
                ClearProfileDetails();
                ClearAvatarList();
            }

            _lastSceneId = sceneId;
        }

        private async void OnProfileRefresh(object? sender, EventArgs e)
        {
            if (InvokeRequired) {
                Invoke(() => OnProfileRefresh(sender, e));
                return;
            }

            if (_lastSelectedAvatarGuid == 0)
                return;

            await PopulateAvatarList();
            await ChangeRelicDetailDisplay();
        }

        private void OnProfileConflict(object? sender, string message)
        {
            if (InvokeRequired) {
                Invoke(() => OnProfileConflict(sender, message));
                return;
            }

            labelConflict.Text = message;
            btnConfirmConflict.Visible = true;
        }

        private void ClearAvatarList()
        {
            flpAvatars.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(c => c.Dispose());
            flpAvatars.Controls.Clear();
        }

        private void ClearProfileDetails()
        {
            flpProfileDetails.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(c => c.Dispose());
            flpProfileDetails.Controls.Clear();
        }

    }
}
