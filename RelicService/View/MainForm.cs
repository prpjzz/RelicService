using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RelicService.Data;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Event;
using RelicService.Properties;
using RelicService.Service;
using RelicService.Tools;

namespace RelicService.View;

internal class MainForm : Form
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

    private bool _isOnline;

    private bool _isOffline = true;

    private uint _lastSceneId;

    private ulong _lastSelectedAvatarGuid;

    private readonly Dictionary<ulong, AvatarProfileItem> _avatarProfileItemMap = new Dictionary<ulong, AvatarProfileItem>();

    private readonly Dictionary<uint, Image> _avatarImageCache = new Dictionary<uint, Image>();

    private readonly Dictionary<uint, Image> _relicImageCache = new Dictionary<uint, Image>();

    private readonly HashSet<uint> _relicImageRef = new HashSet<uint>();

    private IContainer components;

    private StatusStrip statusStrip;

    private ToolStripStatusLabel serverStatus;

    private MenuStrip menuStrip;

    private ToolStripMenuItem menuAbout;

    private GroupBox groupBox;

    private SplitContainer splitContainer;

    private TableLayoutPanel tableLayoutLeft;

    private Button btnAddPreset;

    private GroupBox groupBoxAvatar;

    private FlowLayoutPanel flpAvatars;

    private GroupBox groupBoxSelectedAvatar;

    private TextBox textBoxAvatarFilter;

    private TableLayoutPanel tableLayoutPanel;

    private Label labelUid;

    private Label labelScene;

    private CheckBox cbAutoEquip;

    private Label labelConflict;

    private Button btnConfirmConflict;

    private FlowLayoutPanel flpProfileDetails;

    private CheckBox cbEnableGameMessage;

    private Button btnRefreshMetadata;

    public MainForm()
    {
    }

    public MainForm(EventManager eventManager, ResourceManager resourceManager, StatusService statusService, AutoEquipService autoEquipService, GameMessageService gameMessageService, Network network, ApiService apiService, SqliteContext dbContext)
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

    private void MainForm_Load(object sender, EventArgs e)
    {
        SetupBindings();
        labelConflict.Text = string.Empty;
        btnConfirmConflict.Visible = false;
        using Graphics graphics = CreateGraphics();
        Program.DpiScaleFactor = graphics.DpiX / 96f;
        Task.Run((Func<Task?>)CheckVersion);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _eventManager.FireEventAsync(EventId.EvtShutdown);
    }

    private async void btnAddPreset_Click(object sender, EventArgs e)
    {
        using AddPresetForm form = Program.ServiceProvider.GetRequiredService<AddPresetForm>();
        form.ShowDialog();
        await PopulateAvatarList();
        await ChangeRelicDetailDisplay();
    }

    private void btnConfirmConflict_Click(object sender, EventArgs e)
    {
        labelConflict.Text = string.Empty;
        btnConfirmConflict.Visible = false;
    }

    private void menuAbout_Click(object sender, EventArgs e)
    {
        Program.ServiceProvider.GetRequiredService<AboutForm>().ShowDialog();
    }

    private void textBoxAvatarFilter_TextChanged(object sender, EventArgs e)
    {
        string text = textBoxAvatarFilter.Text;
        foreach (object control in flpAvatars.Controls)
        {
            if (control is AvatarProfileItem avatarProfileItem)
            {
                avatarProfileItem.Visible = avatarProfileItem.AvatarName.Contains(text) || string.IsNullOrWhiteSpace(text);
            }
        }
    }

    private async Task CheckVersion()
    {
        try
        {
            string text = await _network.GetVersionInfo();
            if (text == null)
            {
                return;
            }
            VersionInfo versionInfo = JsonConvert.DeserializeObject<VersionInfo>(text);
            if (versionInfo == null || versionInfo.Build <= Program.Build)
            {
                return;
            }
            Invoke(delegate
            {
                if (MessageBox.Show("A new version is available. Do you want to download it?", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://space.bilibili.com/44434084/dynamic",
                        UseShellExecute = true
                    });
                }
            });
        }
        catch (Exception)
        {
        }
    }

    private void SetupBindings()
    {
        btnAddPreset.Enabled = _isOnline;
        cbAutoEquip.DataBindings.Add("Checked", _autoEquipService, "Enabled", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
        cbEnableGameMessage.DataBindings.Add("Checked", _gameMessageService, "Enabled", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private async Task PopulateAvatarList()
    {
        _avatarProfileItemMap.Clear();
        _lastSelectedAvatarGuid = 0uL;
        ClearAvatarList();
        await Task.Run(async delegate
        {
            while (_statusService.CurrentUid == 0)
            {
                await Task.Delay(100);
            }
        });
        List<DbUserAvatar> list = (from a in (await (from p in _dbContext.RelicProfiles.Include((DbRelicProfile p) => p.UserAvatar).ThenInclude((DbUserAvatar ua) => ua.Avatar)
                                                     where p.UserAvatar.UserUid == _statusService.CurrentUid
                                                     select p).ToListAsync()).Select((DbRelicProfile p) => p.UserAvatar).Distinct()
                                   orderby a.AvatarId
                                   select a).ToList();
        foreach (DbUserAvatar userAvatar in list)
        {
            if (!_avatarImageCache.TryGetValue(userAvatar.AvatarId, out Image value))
            {
                value = await _resourceManager.GetAvatarImage(userAvatar.AvatarId);
                if (value == null)
                {
                    continue;
                }
                value = Utils.ResizeImage(value, 65, 65);
                _avatarImageCache.Add(userAvatar.AvatarId, value);
            }
            AvatarProfileItem avatarProfileItem = new AvatarProfileItem(userAvatar.Guid);
            avatarProfileItem.AvatarImage = value;
            avatarProfileItem.AvatarName = userAvatar.Avatar.Name;
            avatarProfileItem.OnClickCallback = OnAvatarProfileItemClicked;
            _avatarProfileItemMap.TryAdd(userAvatar.Guid, avatarProfileItem);
            flpAvatars.Controls.Add(avatarProfileItem);
        }
    }

    private async void OnAvatarProfileItemClicked(ulong avatarGuid)
    {
        if (_lastSelectedAvatarGuid != 0L)
        {
            _avatarProfileItemMap[_lastSelectedAvatarGuid].IsSelected = false;
        }
        _avatarProfileItemMap[avatarGuid].IsSelected = true;
        _lastSelectedAvatarGuid = avatarGuid;
        await ChangeRelicDetailDisplay();
    }

    private async Task ChangeRelicDetailDisplay()
    {
        flpProfileDetails.Controls.OfType<Control>().ToList().ForEach(delegate (Control c)
        {
            c.Dispose();
        });
        flpProfileDetails.Controls.Clear();
        _relicImageRef.ToList().ForEach(_resourceManager.FreeRelicImage);
        _relicImageRef.Clear();
        List<DbRelicProfile> list = await (from p in _dbContext.RelicProfiles.Include((DbRelicProfile p) => p.UserAvatar).ThenInclude((DbUserAvatar ua) => ua.Avatar).Include((DbRelicProfile p) => p.RelicItems)
                .ThenInclude((DbRelicItem r) => r.Relic)
                .Include((DbRelicProfile p) => p.RelicItems)
                .ThenInclude((DbRelicItem r) => r.Affixes)
                .Include((DbRelicProfile p) => p.TeamContexts)
                                           where p.UserAvatar.Guid == _lastSelectedAvatarGuid
                                           select p).ToListAsync();
        if (list.Count == 0)
        {
            return;
        }
        DbUserAvatar userAvatar = list.First().UserAvatar;
        groupBoxSelectedAvatar.Text = userAvatar.Avatar.Name;
        foreach (DbRelicProfile item in list)
        {
            ProfileDetails value = await CreateProfileDetailPanel(item);
            flpProfileDetails.Controls.Add(value);
        }
        CheckProfileDetailSize();
    }

    private async Task<ProfileDetails> CreateProfileDetailPanel(DbRelicProfile profile)
    {
        ProfileDetails panel = Program.ServiceProvider.GetRequiredService<ProfileDetails>();
        panel.ProfileName = profile.ProfileName;
        panel.RelicProfile = profile;
        panel.Dock = DockStyle.Top;
        _ = flpProfileDetails.ClientSize.Height;
        int num = flpProfileDetails.Width - flpProfileDetails.Margin.Horizontal;
        panel.MinimumSize = new Size(num, (int)((float)num * ProfileDetails.WidthToHeightScale));
        foreach (DbRelicItem relic in profile.RelicItems)
        {
            if (!_relicImageCache.TryGetValue(relic.ItemId, out Image value))
            {
                value = await _resourceManager.GetRelicImage(relic.ItemId);
                if (value == null)
                {
                    continue;
                }
                value = Utils.ResizeImage(value, 80, 80);
                _relicImageCache.Add(relic.ItemId, value);
                _relicImageRef.Add(relic.ItemId);
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Utils.FormatFightPropShort(relic.MainPropType, relic.MainPropValue));
            stringBuilder.AppendLine();
            foreach (DbRelicAffix affix in relic.Affixes)
            {
                string value2 = Utils.FormatFightPropShort(affix.PropType, affix.PropValue);
                stringBuilder.AppendLine(value2);
            }
            switch (relic.EquipType)
            {
                case EquipType.EQUIP_BRACER:
                    panel.FlowerImage = value;
                    panel.FlowerLabel = stringBuilder.ToString();
                    break;
                case EquipType.EQUIP_NECKLACE:
                    panel.FeatherImage = value;
                    panel.FeatherLabel = stringBuilder.ToString();
                    break;
                case EquipType.EQUIP_SHOES:
                    panel.SandImage = value;
                    panel.SandLabel = stringBuilder.ToString();
                    break;
                case EquipType.EQUIP_RING:
                    panel.GobletImage = value;
                    panel.GobletLabel = stringBuilder.ToString();
                    break;
                case EquipType.EQUIP_DRESS:
                    panel.HeadImage = value;
                    panel.HeadLabel = stringBuilder.ToString();
                    break;
            }
        }
        panel.ConflicLabel = "";
        ProfileDetails profileDetails = panel;
        profileDetails.ConflicLabel = await CheckProfileConflict(profile);
        return panel;
    }

    private async Task<string> CheckProfileConflict(DbRelicProfile profile)
    {
        if (profile.WithScene.Count == 0 && profile.TeamContexts.Count == 0)
        {
            return string.Empty;
        }
        List<DbRelicProfile> otherProfiles2 = await (from p in _dbContext.RelicProfiles.Include((DbRelicProfile p) => p.RelicItems).ThenInclude((DbRelicItem r) => r.Relic).Include((DbRelicProfile p) => p.UserAvatar)
                .ThenInclude((DbUserAvatar ua) => ua.Avatar)
                .Include((DbRelicProfile p) => p.TeamContexts)
                                                     where p.Id != profile.Id && p.UserAvatar.UserUid == profile.UserAvatar.UserUid
                                                     select p).ToListAsync();
        ConcurrentBag<DbRelicProfile> teamConflicts = new ConcurrentBag<DbRelicProfile>();
        ConcurrentBag<DbRelicProfile> sceneConflicts = new ConcurrentBag<DbRelicProfile>();
        await Task.Run(delegate
        {
            Parallel.ForEach(otherProfiles2, delegate (DbRelicProfile otherProfiles)
            {
                if (ProfileHasTeamConflict(profile, otherProfiles))
                {
                    teamConflicts.Add(otherProfiles);
                }
                if (ProfileHasSceneConflict(profile, otherProfiles))
                {
                    sceneConflicts.Add(otherProfiles);
                }
            });
        });
        if (teamConflicts.Count == 0 && sceneConflicts.Count == 0)
        {
            return string.Empty;
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Conflict:");
        if (sceneConflicts.Count > 0)
        {
            stringBuilder.Append("Scene(");
            stringBuilder.Append(sceneConflicts.Select((DbRelicProfile p) => p.ProfileName).Aggregate((string a, string b) => a + "," + b));
            stringBuilder.Append(") ");
        }
        if (teamConflicts.Count > 0)
        {
            stringBuilder.Append("队伍(");
            stringBuilder.Append(teamConflicts.Select((DbRelicProfile p) => p.ProfileName).Aggregate((string a, string b) => a + "," + b));
            stringBuilder.Append(") ");
        }
        return stringBuilder.ToString();
    }

    private bool ProfileHasTeamConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
    {
        IEnumerable<ulong> first = profile.RelicItems.Select((DbRelicItem r) => r.Guid);
        IEnumerable<ulong> second = otherProfile.RelicItems.Select((DbRelicItem r) => r.Guid);
        if (first.Intersect(second).Any())
        {
            foreach (DbRelicProfileTeamContext teamContext in profile.TeamContexts)
            {
                foreach (DbRelicProfileTeamContext teamContext2 in otherProfile.TeamContexts)
                {
                    if (teamContext.AvatarIds.Intersect(teamContext2.AvatarIds).Any())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ProfileHasSceneConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
    {
        if (profile.UserAvatar.Guid != otherProfile.UserAvatar.Guid)
        {
            return false;
        }
        return profile.WithScene.Intersect(otherProfile.WithScene).Any();
    }

    private void CheckProfileDetailSize()
    {
        List<ProfileDetails> list = flpProfileDetails.Controls.OfType<ProfileDetails>().ToList();
        flpProfileDetails.Controls.OfType<ScrollableControl>().FirstOrDefault();
        int minWidth = flpProfileDetails.ClientSize.Width - flpProfileDetails.Margin.Horizontal;
        if (flpProfileDetails.VerticalScroll.Visible)
        {
            minWidth = flpProfileDetails.Width - SystemInformation.VerticalScrollBarWidth - flpProfileDetails.Margin.Horizontal;
        }
        list.ForEach(delegate (ProfileDetails x)
        {
            x.MinimumSize = new Size(minWidth, (int)((float)minWidth * ProfileDetails.WidthToHeightScale));
        });
    }

    private void OnServiceStatusChanged(object? sender, bool isOnline)
    {
        if (base.InvokeRequired)
        {
            Invoke(delegate
            {
                OnServiceStatusChanged(sender, isOnline);
            });
            return;
        }
        _isOnline = isOnline;
        _isOffline = !isOnline;
        btnAddPreset.Enabled = _isOnline;
        serverStatus.Text = (_isOnline ? "Connected" : "Not Connected");
        serverStatus.Image = (_isOnline ? Resources.dot_green : Resources.dot_red);
        if (_isOffline)
        {
            _lastSelectedAvatarGuid = 0uL;
            ClearAvatarList();
            ClearProfileDetails();
            labelUid.Text = "UID: ";
            labelScene.Text = "Scene: ";
        }
    }

    private void UpdateUidText(uint uid)
    {
        string text = uid.ToString();
        if (text.Length > 3)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            for (int i = 2; i < text.Length - 2; i++)
            {
                stringBuilder[i] = '*';
            }
            text = stringBuilder.ToString();
        }
        labelUid.Text = "UID: " + text;
    }

    private async void OnUidChanged(object? sender, uint uid)
    {
        if (base.InvokeRequired)
        {
            Invoke(delegate
            {
                OnUidChanged(sender, uid);
            });
            return;
        }
        UpdateUidText(uid);
        if (uid == 0)
        {
            _avatarProfileItemMap.Clear();
            ClearProfileDetails();
        }
        else
        {
            _lastSelectedAvatarGuid = 0uL;
            await PopulateAvatarList();
            ClearProfileDetails();
        }
    }

    private void OnSceneChanged(object? sender, uint sceneId)
    {
        if (base.InvokeRequired)
        {
            Invoke(delegate
            {
                OnSceneChanged(sender, sceneId);
            });
            return;
        }
        labelScene.Text = "Scene: " + sceneId;
        if (sceneId == 0)
        {
            ClearProfileDetails();
            ClearAvatarList();
        }
        _lastSceneId = sceneId;
    }

    private async void OnProfileRefresh(object? sender, EventArgs e)
    {
        if (base.InvokeRequired)
        {
            Invoke(delegate
            {
                OnProfileRefresh(sender, e);
            });
        }
        else if (_lastSelectedAvatarGuid != 0L)
        {
            await PopulateAvatarList();
            await ChangeRelicDetailDisplay();
        }
    }

    private void OnProfileConflict(object? sender, string message)
    {
        if (base.InvokeRequired)
        {
            Invoke(delegate
            {
                OnProfileConflict(sender, message);
            });
        }
        else
        {
            labelConflict.Text = message;
            btnConfirmConflict.Visible = true;
        }
    }

    private void ClearAvatarList()
    {
        flpAvatars.Controls.OfType<Control>().ToList().ForEach(delegate (Control c)
        {
            c.Dispose();
        });
        flpAvatars.Controls.Clear();
    }

    private void ClearProfileDetails()
    {
        flpProfileDetails.Controls.OfType<Control>().ToList().ForEach(delegate (Control c)
        {
            c.Dispose();
        });
        flpProfileDetails.Controls.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
        statusStrip = new StatusStrip();
        serverStatus = new ToolStripStatusLabel();
        menuStrip = new MenuStrip();
        menuAbout = new ToolStripMenuItem();
        groupBox = new GroupBox();
        splitContainer = new SplitContainer();
        tableLayoutLeft = new TableLayoutPanel();
        groupBoxAvatar = new GroupBox();
        flpAvatars = new FlowLayoutPanel();
        btnAddPreset = new Button();
        textBoxAvatarFilter = new TextBox();
        tableLayoutPanel = new TableLayoutPanel();
        groupBoxSelectedAvatar = new GroupBox();
        flpProfileDetails = new FlowLayoutPanel();
        labelUid = new Label();
        labelScene = new Label();
        cbAutoEquip = new CheckBox();
        labelConflict = new Label();
        btnConfirmConflict = new Button();
        cbEnableGameMessage = new CheckBox();
        btnRefreshMetadata = new Button();
        statusStrip.SuspendLayout();
        menuStrip.SuspendLayout();
        groupBox.SuspendLayout();
        ((ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        tableLayoutLeft.SuspendLayout();
        groupBoxAvatar.SuspendLayout();
        tableLayoutPanel.SuspendLayout();
        groupBoxSelectedAvatar.SuspendLayout();
        SuspendLayout();
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { serverStatus });
        statusStrip.Location = new Point(0, 659);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1264, 22);
        statusStrip.SizingGrip = false;
        statusStrip.TabIndex = 0;
        statusStrip.Text = "statusStrip";
        // 
        // serverStatus
        // 
        serverStatus.Image = (Image)resources.GetObject("serverStatus.Image");
        serverStatus.Name = "serverStatus";
        serverStatus.Size = new Size(104, 17);
        serverStatus.Text = "Not Connected";
        // 
        // menuStrip
        // 
        menuStrip.Items.AddRange(new ToolStripItem[] { menuAbout });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1264, 24);
        menuStrip.TabIndex = 1;
        menuStrip.Text = "menuStrip";
        // 
        // menuAbout
        // 
        menuAbout.Name = "menuAbout";
        menuAbout.Size = new Size(52, 20);
        menuAbout.Text = "About";
        menuAbout.Click += menuAbout_Click;
        // 
        // groupBox
        // 
        groupBox.Controls.Add(splitContainer);
        groupBox.Dock = DockStyle.Fill;
        groupBox.Location = new Point(0, 24);
        groupBox.Name = "groupBox";
        groupBox.Size = new Size(1264, 635);
        groupBox.TabIndex = 2;
        groupBox.TabStop = false;
        groupBox.Text = "Home";
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(3, 19);
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(tableLayoutLeft);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(tableLayoutPanel);
        splitContainer.Size = new Size(1258, 613);
        splitContainer.SplitterDistance = 350;
        splitContainer.TabIndex = 0;
        // 
        // tableLayoutLeft
        // 
        tableLayoutLeft.ColumnCount = 1;
        tableLayoutLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutLeft.Controls.Add(groupBoxAvatar, 0, 2);
        tableLayoutLeft.Controls.Add(btnAddPreset, 0, 0);
        tableLayoutLeft.Controls.Add(textBoxAvatarFilter, 0, 1);
        tableLayoutLeft.Dock = DockStyle.Fill;
        tableLayoutLeft.Location = new Point(0, 0);
        tableLayoutLeft.Name = "tableLayoutLeft";
        tableLayoutLeft.RowCount = 3;
        tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
        tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
        tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
        tableLayoutLeft.Size = new Size(350, 613);
        tableLayoutLeft.TabIndex = 0;
        // 
        // groupBoxAvatar
        // 
        groupBoxAvatar.Controls.Add(flpAvatars);
        groupBoxAvatar.Dock = DockStyle.Fill;
        groupBoxAvatar.Location = new Point(3, 63);
        groupBoxAvatar.Name = "groupBoxAvatar";
        groupBoxAvatar.Size = new Size(344, 547);
        groupBoxAvatar.TabIndex = 1;
        groupBoxAvatar.TabStop = false;
        groupBoxAvatar.Text = "Pre-set Character";
        // 
        // flpAvatars
        // 
        flpAvatars.AutoScroll = true;
        flpAvatars.BackColor = SystemColors.ControlLightLight;
        flpAvatars.BorderStyle = BorderStyle.FixedSingle;
        flpAvatars.Dock = DockStyle.Fill;
        flpAvatars.Location = new Point(3, 19);
        flpAvatars.Name = "flpAvatars";
        flpAvatars.Padding = new Padding(18, 10, 20, 20);
        flpAvatars.Size = new Size(338, 525);
        flpAvatars.TabIndex = 0;
        // 
        // btnAddPreset
        // 
        btnAddPreset.Dock = DockStyle.Fill;
        btnAddPreset.Location = new Point(5, 3);
        btnAddPreset.Margin = new Padding(5, 3, 5, 3);
        btnAddPreset.Name = "btnAddPreset";
        btnAddPreset.Size = new Size(340, 24);
        btnAddPreset.TabIndex = 0;
        btnAddPreset.Text = "Add New Preset";
        btnAddPreset.UseVisualStyleBackColor = true;
        btnAddPreset.Click += btnAddPreset_Click;
        // 
        // textBoxAvatarFilter
        // 
        textBoxAvatarFilter.Dock = DockStyle.Fill;
        textBoxAvatarFilter.Location = new Point(6, 33);
        textBoxAvatarFilter.Margin = new Padding(6, 3, 6, 3);
        textBoxAvatarFilter.Name = "textBoxAvatarFilter";
        textBoxAvatarFilter.PlaceholderText = "Search Character...";
        textBoxAvatarFilter.Size = new Size(338, 23);
        textBoxAvatarFilter.TabIndex = 2;
        textBoxAvatarFilter.TextChanged += textBoxAvatarFilter_TextChanged;
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.ColumnCount = 5;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        tableLayoutPanel.Controls.Add(groupBoxSelectedAvatar, 0, 2);
        tableLayoutPanel.Controls.Add(labelUid, 0, 0);
        tableLayoutPanel.Controls.Add(labelScene, 0, 1);
        tableLayoutPanel.Controls.Add(cbAutoEquip, 1, 1);
        tableLayoutPanel.Controls.Add(labelConflict, 1, 0);
        tableLayoutPanel.Controls.Add(btnConfirmConflict, 4, 0);
        tableLayoutPanel.Controls.Add(cbEnableGameMessage, 2, 1);
        tableLayoutPanel.Controls.Add(btnRefreshMetadata, 3, 1);
        tableLayoutPanel.Dock = DockStyle.Top;
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
        tableLayoutPanel.Size = new Size(904, 613);
        tableLayoutPanel.TabIndex = 1;
        // 
        // groupBoxSelectedAvatar
        // 
        tableLayoutPanel.SetColumnSpan(groupBoxSelectedAvatar, 5);
        groupBoxSelectedAvatar.Controls.Add(flpProfileDetails);
        groupBoxSelectedAvatar.Dock = DockStyle.Fill;
        groupBoxSelectedAvatar.Location = new Point(3, 63);
        groupBoxSelectedAvatar.Name = "groupBoxSelectedAvatar";
        groupBoxSelectedAvatar.Size = new Size(898, 547);
        groupBoxSelectedAvatar.TabIndex = 0;
        groupBoxSelectedAvatar.TabStop = false;
        groupBoxSelectedAvatar.Text = "Presets";
        // 
        // flpProfileDetails
        // 
        flpProfileDetails.AutoScroll = true;
        flpProfileDetails.BackColor = SystemColors.Window;
        flpProfileDetails.BorderStyle = BorderStyle.FixedSingle;
        flpProfileDetails.Dock = DockStyle.Fill;
        flpProfileDetails.FlowDirection = FlowDirection.TopDown;
        flpProfileDetails.Location = new Point(3, 19);
        flpProfileDetails.Name = "flpProfileDetails";
        flpProfileDetails.Size = new Size(892, 525);
        flpProfileDetails.TabIndex = 0;
        flpProfileDetails.WrapContents = false;
        // 
        // labelUid
        // 
        labelUid.AutoSize = true;
        labelUid.Location = new Point(2, 7);
        labelUid.Margin = new Padding(2, 7, 7, 7);
        labelUid.Name = "labelUid";
        labelUid.Size = new Size(32, 15);
        labelUid.TabIndex = 1;
        labelUid.Text = "UID: ";
        // 
        // labelScene
        // 
        labelScene.AutoSize = true;
        labelScene.Location = new Point(2, 37);
        labelScene.Margin = new Padding(2, 7, 7, 7);
        labelScene.Name = "labelScene";
        labelScene.Size = new Size(41, 15);
        labelScene.TabIndex = 3;
        labelScene.Text = "Scene:";
        // 
        // cbAutoEquip
        // 
        cbAutoEquip.AutoSize = true;
        cbAutoEquip.Location = new Point(53, 36);
        cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
        cbAutoEquip.Name = "cbAutoEquip";
        cbAutoEquip.Size = new Size(85, 19);
        cbAutoEquip.TabIndex = 4;
        cbAutoEquip.Text = "Auto Equip";
        cbAutoEquip.UseVisualStyleBackColor = true;
        // 
        // labelConflict
        // 
        labelConflict.AutoSize = true;
        tableLayoutPanel.SetColumnSpan(labelConflict, 3);
        labelConflict.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelConflict.ForeColor = Color.DarkRed;
        labelConflict.Location = new Point(52, 7);
        labelConflict.Margin = new Padding(2, 7, 7, 7);
        labelConflict.Name = "labelConflict";
        labelConflict.Size = new Size(40, 15);
        labelConflict.TabIndex = 5;
        labelConflict.Text = "label1";
        // 
        // btnConfirmConflict
        // 
        btnConfirmConflict.Location = new Point(827, 3);
        btnConfirmConflict.Name = "btnConfirmConflict";
        btnConfirmConflict.Size = new Size(74, 23);
        btnConfirmConflict.TabIndex = 6;
        btnConfirmConflict.Text = "Confirm";
        btnConfirmConflict.UseVisualStyleBackColor = true;
        btnConfirmConflict.Click += btnConfirmConflict_Click;
        // 
        // cbEnableGameMessage
        // 
        cbEnableGameMessage.AutoSize = true;
        cbEnableGameMessage.Location = new Point(144, 36);
        cbEnableGameMessage.Margin = new Padding(3, 6, 3, 3);
        cbEnableGameMessage.Name = "cbEnableGameMessage";
        cbEnableGameMessage.Size = new Size(178, 19);
        cbEnableGameMessage.TabIndex = 7;
        cbEnableGameMessage.Text = "Enable in-game notifications";
        cbEnableGameMessage.UseVisualStyleBackColor = true;
        // 
        // btnRefreshMetadata
        // 
        btnRefreshMetadata.AutoSize = true;
        btnRefreshMetadata.Location = new Point(328, 33);
        btnRefreshMetadata.Name = "btnRefreshMetadata";
        btnRefreshMetadata.Size = new Size(109, 24);
        btnRefreshMetadata.TabIndex = 8;
        btnRefreshMetadata.Text = "Refresh Metadata";
        btnRefreshMetadata.UseVisualStyleBackColor = true;
        btnRefreshMetadata.Visible = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1264, 681);
        Controls.Add(groupBox);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = menuStrip;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Relic Service by: Ex_M";
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        groupBox.ResumeLayout(false);
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        tableLayoutLeft.ResumeLayout(false);
        tableLayoutLeft.PerformLayout();
        groupBoxAvatar.ResumeLayout(false);
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        groupBoxSelectedAvatar.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
