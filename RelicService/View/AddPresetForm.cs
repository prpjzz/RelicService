using Microsoft.EntityFrameworkCore;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;
using RelicService.Service;
using RelicService.View;
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
    internal partial class AddPresetForm : Form
    {
        private static readonly Size TeamInfoPanelSize = new Size(475, 80);
        private static readonly Size AllInfoPanelSize = new Size(455, 75);

        private readonly AvatarService _avatarService;
        private readonly EquipService _equipService;
        private readonly StatusService _statusService;
        private readonly EventManager _eventManager;
        private readonly ResourceManager _resourceManager;
        private readonly SqliteContext _dbContext;

        private readonly Dictionary<ulong, AvatarRelicInfo> _teamAvatarControlsMap = new();
        private readonly Dictionary<ulong, AvatarRelicInfo> _allAvatarControlsMap = new();

        private ulong _lastClickedTeamAvatar = 0;
        private ulong _lastClickedAllAvatar = 0;

        private readonly Dictionary<ulong, DbUserAvatar> _avatarDataDict = new();
        private List<DbUserAvatar> _checkedListCache = new();
        private List<DbUserAvatar> _currentTeamCache = new();

        private readonly Dictionary<uint, Image> _avatarImageCache = new();
        private readonly Dictionary<uint, Image> _relicImageCache = new();

        private readonly HashSet<uint> _avatarImageRef = new();
        private readonly HashSet<uint> _relicImageRef = new();

        public AddPresetForm() { }

        public AddPresetForm(
            AvatarService avatarService,
            EquipService equipService,
            StatusService statusService,
            EventManager eventManager,
            ResourceManager resourceManager,
            SqliteContext dbContext
            )
        {
            InitializeComponent();
            _avatarService = avatarService;
            _equipService = equipService;
            _statusService = statusService;
            _eventManager = eventManager;
            _resourceManager = resourceManager;
            _dbContext = dbContext;


            _eventManager.OnFetchProgress += OnFetechProgressEvent;
            _eventManager.OnUidChanged += OnUidChanged;
        }

        #region Form Events

        private void AddPresetForm_Load(object sender, EventArgs e)
        {
            SetupBindings();

            // disable tab control so that it won't cause the tabs to have duplicate data
            // due to the trigger of tabControl_SelectedIndexChanged
            tabControl.Enabled = false;

            statusLabel.Text = @"启动中...";
        }

        private async void AddPresetForm_Shown(object sender, EventArgs e)
        {
            await OnSwitchToTeamTab();
            await PopulateAvatarCheckboxList();
            tabControl.Enabled = true;
        }

        private void AddPresetForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _eventManager.OnFetchProgress -= OnFetechProgressEvent;
            _eventManager.OnUidChanged -= OnUidChanged;
            _avatarImageRef
                .ToList()
                .ForEach(_resourceManager.FreeAvatarImage);

            _relicImageRef
                .ToList()
                .ForEach(_resourceManager.FreeRelicImage);
        }

        private async void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl.Enabled = false;
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    await OnSwitchToTeamTab();
                    break;
                case 1:
                    await OnSwitchToAllAvatarsTab();
                    break;
                default:
                    break;
            }
            tabControl.Enabled = true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var profileName = textBoxProfileName.Text;
                var autoEquip = cbEnableAutoEquip.Checked;
                var activeWithScene = cbActiveWithScene.Checked;
                var sceneIds = tbSceneIds.Text;
                var activeWithTeam = cbActiveWithTeam.Checked;

                await CreateAndSaveConfig(profileName, autoEquip, activeWithScene, sceneIds, activeWithTeam);
                await _dbContext.SaveChangesAsync();

                MessageBox.Show(@"保存成功", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _ = Task.Run(async () =>
                {
                    await Task.Delay(800);
                    Invoke(() => { statusLabel.Text = "保存成功"; });
                    await Task.Delay(2000);
                    Invoke(() => { statusLabel.Text = "OK"; });
                });

                textBoxProfileName.Text = "";
                cbEnableAutoEquip.Checked = false;
                cbActiveWithScene.Checked = false;
                tbSceneIds.Text = "";
                cbActiveWithTeam.Checked = false;
                btnClearSelectedTeam_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefreshTeam_Click(object sender, EventArgs e)
        {
            flpCurrentTeam.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(x => x.Dispose());

            flpCurrentTeam.Controls.Clear();
            _teamAvatarControlsMap.Clear();
            tabControl.Enabled = false;
            await OnSwitchToTeamTab();
            tabControl.Enabled = true;
        }

        private async void btnRefreshAllAvatar_Click(object sender, EventArgs e)
        {
            flpAllAvatar.Controls
                .OfType<Control>()
                .ToList()
                .ForEach(x => x.Dispose());

            flpAllAvatar.Controls.Clear();
            _allAvatarControlsMap.Clear();
            tabControl.Enabled = false;
            await OnSwitchToAllAvatarsTab();
            tabControl.Enabled = true;
        }

        private void btnUseCurrentTeam_Click(object sender, EventArgs e)
        {
            var controls = flpAvatars.Controls
                .OfType<CheckBox>()
                .ToList();

            var teamAvatarGuidList = _currentTeamCache
                .Where(x => x.Guid != _lastClickedTeamAvatar)
                .Select(x => x.Guid)
                .ToList();

            foreach (var control in controls)
            {
                control.Checked = teamAvatarGuidList.Contains((ulong)control.Tag);
            }
        }

        private void btnClearSelectedTeam_Click(object sender, EventArgs e)
        {
            var controls = flpAvatars.Controls
                .OfType<CheckBox>()
                .ToList();

            controls.ForEach(x => x.Checked = false);
        }

        private void tbTeammateSearch_TextChanged(object sender, EventArgs e)
        {
            var controls = flpAvatars.Controls
                .OfType<CheckBox>()
                .ToList();

            if (string.IsNullOrWhiteSpace(tbTeammateSearch.Text))
            {
                controls.ForEach(x => x.Visible = true);
                return;
            }

            controls.ForEach(x => x.Visible = x.Text.Contains(tbTeammateSearch.Text));
        }

        private void flpAvatars_OnCheckedChanged(object? sender, EventArgs e)
        {
            var controls = flpAvatars.Controls
                .OfType<CheckBox>()
                .ToList();

            var checkedCount = controls.Count(x => x.Checked);
            // disables unchecked checkboxes if checked count is 3 or more
            // and enables all checkboxes if checked count is less than 3
            controls.ForEach(x => x.Enabled = checkedCount >= 3 ? x.Checked : true);

            // sets the selected text
            if (checkedCount > 0)
            {
                var text = controls
                    .Where(x => x.Checked)
                    .Select(x => x.Text)
                    .Aggregate((x, y) => $"{x},{y}");

                labelSelectedTeammates.Text = text;
            }
            else
            {
                labelSelectedTeammates.Text = "无";
            }

        }

        #endregion

        private void SetupBindings()
        {
            // disable profile config before an item is selected
            groupBoxRight.Enabled = false;

            cbActiveWithScene.DataBindings.Add("Enabled", cbEnableAutoEquip, "Checked");
            labelSceneId.DataBindings.Add("Enabled", cbActiveWithScene, "Checked");
            tbSceneIds.DataBindings.Add("Enabled", cbActiveWithScene, "Checked");

            cbActiveWithTeam.DataBindings.Add("Enabled", cbEnableAutoEquip, "Checked");
            labelTeammates.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
            labelSelectedTeammates.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
            btnUseCurrentTeam.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
            btnClearSelectedTeam.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
            tbTeammateSearch.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
            flpAvatars.DataBindings.Add("Enabled", cbActiveWithTeam, "Checked");
        }

        private void OnFetechProgressEvent(object? sender, FetchProgressEvent e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnFetechProgressEvent(sender, e)));
                return;
            }

            if (e.Type == FetchType.None)
            {
                statusLabel.Text = "OK";
                return;
            }

            var sb = new StringBuilder();
            sb.Append("正在获取");
            sb.Append(e.Name);

            if (e.Total > 0)
            {
                sb.Append(" (");
                sb.Append(e.Current);
                sb.Append("/");
                sb.Append(e.Total);
                sb.Append(")");
            }

            statusLabel.Text = sb.ToString();

        }

        private void OnUidChanged(object? sender, uint uid)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnUidChanged(sender, uid));
                return;
            }

            Close();
        }

        private async Task PopulateAvatarCheckboxList()
        {
            if (_statusService.CurrentUid == 0)
                return;

            var avatars = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
            if (avatars == null || avatars.Count <= 4)
            {
                await _avatarService.UpdateAllAvatarFromGame();
                avatars = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
            }

            if (avatars == null)
                return;

            avatars = avatars.OrderBy(x => x.AvatarId).ToList();
            _checkedListCache = avatars;

            flpAvatars.Controls.Clear();
            foreach (var avatar in _checkedListCache)
            {
                var checkbox = new CheckBox();
                checkbox.Text = avatar.Avatar.Name;
                checkbox.Tag = avatar.Guid;
                checkbox.Margin = new Padding(3, 3, 3, 3);
                checkbox.CheckedChanged += flpAvatars_OnCheckedChanged;
                flpAvatars.Controls.Add(checkbox);
                _avatarDataDict.Add(avatar.Guid, avatar);
            }
        }

        private async Task OnSwitchToTeamTab()
        {
            // only enable the use current team button if we are on the team tab
            btnUseCurrentTeam.Visible = true;
            groupBoxRight.Enabled = _lastClickedTeamAvatar != 0;

            if (flpCurrentTeam.Controls.Count > 0)
                return;

            _lastClickedTeamAvatar = 0;

            await _avatarService.UpdateTeamFromGame();

            var currentTeam = await _avatarService.GetCurrentTeam();
            if (currentTeam == null)
            {
                statusLabel.Text = "未获取到当前队伍信息";
                return;
            }

            var teamAvatarGuidList = currentTeam
                .Select(x => x.Guid)
                .ToList();

            var equipDict = new Dictionary<ulong, List<Data.Dto.RelicDataDto>>();
            foreach (var avatarGuid in teamAvatarGuidList)
            {
                var equip = await _equipService.UpdateAvatarEquipFromGame(avatarGuid);
                if (equip == null)
                    continue;

                equipDict.Add(avatarGuid, equip);
            }

            foreach (var avatar in currentTeam)
            {
                var panel = await CreateInfoPanel(avatar, equipDict);
                if (panel == null)
                    continue;

                panel.Size = TeamInfoPanelSize;
                panel.OnClickCallback = OnTeamAvatarEntryClick;

                var minWidth = flpCurrentTeam.ClientSize.Width - flpCurrentTeam.Margin.Horizontal;
                panel.MinimumSize = new Size(minWidth, (int)(minWidth * 0.16666)); // height is the ratio of the original size 475x80

                _teamAvatarControlsMap.Add(avatar.Guid, panel);
                flpCurrentTeam.Controls.Add(panel);
            }

            _currentTeamCache = currentTeam;
        }

        private async Task OnSwitchToAllAvatarsTab()
        {
            // all avatars doesn't have information about the current team
            btnUseCurrentTeam.Visible = false;
            groupBoxRight.Enabled = _lastClickedAllAvatar != 0;

            if (flpAllAvatar.Controls.Count > 0)
                return;

            if (_statusService.CurrentUid == 0)
                return;

            _lastClickedAllAvatar = 0;

            await _avatarService.UpdateAllAvatarFromGame();

            var avatars = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
            if (avatars == null)
            {
                statusLabel.Text = "未获取到当前用户人物信息";
                return;
            }

            avatars = avatars.OrderBy(x => x.AvatarId).ToList();

            var avatarGuidList = avatars
                .Select(x => x.Guid)
                .ToList();

            _eventManager.OnFetchProgress -= OnFetechProgressEvent;

            List<Task<(ulong, List<RelicDataDto>)>> tasks = new();
            var equipDict = new Dictionary<ulong, List<Data.Dto.RelicDataDto>>();
            int processedCount = 1;
            foreach (var avatarGuid in avatarGuidList)
            {
                statusLabel.Text = $"正在获取人物装备信息 ({processedCount}/{avatarGuidList.Count})";
                var equip = await _equipService.UpdateAvatarEquipFromGame(avatarGuid);
                if (equip != null)
                    equipDict.Add(avatarGuid, equip);

                processedCount++;
            }

            foreach (var avatar in avatars)
            {
                var panel = await CreateInfoPanel(avatar, equipDict);
                if (panel != null)
                {
                    panel.Size = AllInfoPanelSize;
                    panel.OnClickCallback = OnAllAvatarEntryClick;

                    var minWidth = flpAllAvatar.Width - SystemInformation.VerticalScrollBarWidth - flpAllAvatar.Margin.Horizontal;
                    panel.MinimumSize = new Size(minWidth, (int)(minWidth * 0.16666)); // height is the ratio of the original size 900x150

                    _allAvatarControlsMap.Add(avatar.Guid, panel);
                    flpAllAvatar.Controls.Add(panel);
                }

            }

            statusLabel.Text = "OK";
            _eventManager.OnFetchProgress += OnFetechProgressEvent;
        }

        private async Task<AvatarRelicInfo?> CreateInfoPanel(DbUserAvatar avatar, Dictionary<ulong, List<Data.Dto.RelicDataDto>> equipDict)
        {
            if (!_avatarImageCache.TryGetValue(avatar.AvatarId, out var avatarImage))
            {
                avatarImage = await _resourceManager.GetAvatarImage(avatar.AvatarId);
                if (avatarImage == null)
                    return null;


                avatarImage = Utils.ResizeImage(avatarImage, 70, 70);
                _avatarImageRef.Add(avatar.AvatarId);
                _avatarImageCache.Add(avatar.AvatarId, avatarImage);
            }


            var panel = new AvatarRelicInfo(avatar.Guid);
            panel.AvatarImage = avatarImage;
            panel.Dock = DockStyle.Top;

            foreach (var relic in equipDict[avatar.Guid])
            {
                if (!_relicImageCache.TryGetValue(relic.ItemId, out var relicImage))
                {
                    relicImage = await _resourceManager.GetRelicImage(relic.ItemId);
                    if (relicImage == null)
                        continue;

                    relicImage = Utils.ResizeImage(relicImage, 70, 70);
                    _relicImageRef.Add(relic.ItemId);
                    _relicImageCache.Add(relic.ItemId, relicImage);
                }

                var tooltip = GetPropStatTooltip(relic);

                switch (relic.EquipType)
                {
                    case EquipType.EQUIP_BRACER:
                        panel.FlowerImage = relicImage;
                        panel.FlowerTooltip = tooltip;
                        break;
                    case EquipType.EQUIP_NECKLACE:
                        panel.FeatherImage = relicImage;
                        panel.FeatherTooltip = tooltip;
                        break;
                    case EquipType.EQUIP_SHOES:
                        panel.SandImage = relicImage;
                        panel.SandTooltip = tooltip;
                        break;
                    case EquipType.EQUIP_RING:
                        panel.GobletImage = relicImage;
                        panel.GobletTooltip = tooltip;
                        break;
                    case EquipType.EQUIP_DRESS:
                        panel.HeadImage = relicImage;
                        panel.HeadTooltip = tooltip;
                        break;
                }

            }

            return panel;
        }

        private void OnTeamAvatarEntryClick(ulong avatarGuid)
        {
            if (_lastClickedTeamAvatar != 0)
            {
                var control = _teamAvatarControlsMap[_lastClickedTeamAvatar];
                control.IsSelected = false;
            }

            var clickedControl = _teamAvatarControlsMap[avatarGuid];
            clickedControl.IsSelected = true;

            _lastClickedTeamAvatar = avatarGuid;

            // enable the profile config group box
            groupBoxRight.Enabled = true;
        }

        private void OnAllAvatarEntryClick(ulong avatarGuid)
        {
            if (_lastClickedAllAvatar != 0)
            {
                var control = _allAvatarControlsMap[_lastClickedAllAvatar];
                control.IsSelected = false;
            }

            var clickedControl = _allAvatarControlsMap[avatarGuid];
            clickedControl.IsSelected = true;

            _lastClickedAllAvatar = avatarGuid;

            // enable the profile config group box
            groupBoxRight.Enabled = true;
        }

        private string GetPropStatTooltip(Data.Dto.RelicDataDto relic)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(relic.Level - 1);
            sb.Append("级] ");

            var mainProp = Utils.FormatFightProp(relic.MainPropType, relic.MainPropValue);
            sb.Append(mainProp);
            sb.AppendLine();
            sb.AppendLine();

            foreach (var prop in relic.AppendProp)
            {
                var propStr = Utils.FormatFightProp(prop.PropType, prop.PropValue);
                sb.Append(propStr);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private async Task CreateAndSaveConfig(string profileName, bool autoEquip, bool activeWithScene,
            string sceneIds, bool activeWithTeam)
        {

            if (string.IsNullOrWhiteSpace(profileName))
                throw new Exception("预设名不能为空");

            var selectedAvatarGuid = tabControl.SelectedIndex == 0
                ? _lastClickedTeamAvatar
                : _lastClickedAllAvatar;

            var selectedAvatar = _avatarDataDict[selectedAvatarGuid];

            // check if the profile name already exists for the selected avatar
            var profile = await _dbContext.RelicProfiles
                .Where(x => x.ProfileName == profileName && x.AvatarGuid == selectedAvatarGuid)
                .FirstOrDefaultAsync();
            if (profile != null)
                throw new Exception($"[{selectedAvatar.Avatar.Name}] 预设名已存在");

            var avatarRelics = await _equipService.UpdateAndSaveAvatarEquip(selectedAvatarGuid);
            if (avatarRelics.Count == 0)
                throw new Exception("不能创建无圣遗物的预设");


            var relicProfile = new DbRelicProfile();
            relicProfile.ProfileName = profileName;
            relicProfile.UserAvatar = selectedAvatar;
            relicProfile.RelicItems = avatarRelics;

            if (autoEquip)
            {

                if (activeWithScene)
                {
                    var split = sceneIds.Replace(" ", "").Split(',');
                    if (split.Length == 0)
                        throw new Exception("场景ID不能为空");

                    try
                    {
                        var sceneIdList = split.Select(uint.Parse).ToList();
                        relicProfile.WithScene = sceneIdList;
                    }
                    catch (Exception)
                    {
                        throw new Exception("场景ID必须为数字");
                    }

                }

                if (activeWithTeam)
                {
                    var checkedItems = flpAvatars.Controls
                        .OfType<CheckBox>()
                        .ToList()
                        .Where(x => x.Checked)
                        .Select(x => (ulong)x.Tag)
                        .ToList();

                    if (checkedItems.Count > 3)
                        throw new Exception("队伍人数不能超过3人");
                    if (checkedItems.Count == 0)
                        throw new Exception("队伍人数不能为0");

                    var team = checkedItems
                        .Where(_avatarDataDict.ContainsKey)
                        .Select(x => _avatarDataDict[x])
                        .ToList();

                    var teamContext = new DbRelicProfileTeamContext();
                    teamContext.AvatarIds = team.Select(x => x.AvatarId).ToList();
                    teamContext.Profile = relicProfile;

                    _dbContext.RelicProfileTeamContext.Add(teamContext);
                }

                if (activeWithScene || activeWithTeam)
                    relicProfile.AutoEquip = true;

            }

            
            _dbContext.RelicProfiles.Add(relicProfile);
        }


    }
}
