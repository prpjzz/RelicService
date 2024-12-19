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
    private ToolStripMenuItem languageToolStripMenuItem;
    private ToolStripMenuItem 中文简体ToolStripMenuItem;
    private ToolStripMenuItem englishToolStripMenuItem;
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
		flpProfileDetails.Controls.OfType<Control>().ToList().ForEach(delegate(Control c)
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
			Parallel.ForEach(otherProfiles2, delegate(DbRelicProfile otherProfiles)
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
		stringBuilder.Append("冲突:");
		if (sceneConflicts.Count > 0)
		{
			stringBuilder.Append("场景(");
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
		list.ForEach(delegate(ProfileDetails x)
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
			labelScene.Text = "场景: ";
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
		labelScene.Text = "场景: " + sceneId;
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
		flpAvatars.Controls.OfType<Control>().ToList().ForEach(delegate(Control c)
		{
			c.Dispose();
		});
		flpAvatars.Controls.Clear();
	}

	private void ClearProfileDetails()
	{
		flpProfileDetails.Controls.OfType<Control>().ToList().ForEach(delegate(Control c)
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
        languageToolStripMenuItem = new ToolStripMenuItem();
        中文简体ToolStripMenuItem = new ToolStripMenuItem();
        englishToolStripMenuItem = new ToolStripMenuItem();
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
        resources.ApplyResources(statusStrip, "statusStrip");
        statusStrip.Name = "statusStrip";
        statusStrip.SizingGrip = false;
        // 
        // serverStatus
        // 
        resources.ApplyResources(serverStatus, "serverStatus");
        serverStatus.Name = "serverStatus";
        // 
        // menuStrip
        // 
        menuStrip.Items.AddRange(new ToolStripItem[] { menuAbout, languageToolStripMenuItem });
        resources.ApplyResources(menuStrip, "menuStrip");
        menuStrip.Name = "menuStrip";
        // 
        // menuAbout
        // 
        menuAbout.Name = "menuAbout";
        resources.ApplyResources(menuAbout, "menuAbout");
        menuAbout.Click += menuAbout_Click;
        // 
        // languageToolStripMenuItem
        // 
        languageToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 中文简体ToolStripMenuItem, englishToolStripMenuItem });
        languageToolStripMenuItem.Name = "languageToolStripMenuItem";
        resources.ApplyResources(languageToolStripMenuItem, "languageToolStripMenuItem");
        // 
        // 中文简体ToolStripMenuItem
        // 
        中文简体ToolStripMenuItem.Name = "中文简体ToolStripMenuItem";
        resources.ApplyResources(中文简体ToolStripMenuItem, "中文简体ToolStripMenuItem");
        // 
        // englishToolStripMenuItem
        // 
        englishToolStripMenuItem.Name = "englishToolStripMenuItem";
        resources.ApplyResources(englishToolStripMenuItem, "englishToolStripMenuItem");
        // 
        // groupBox
        // 
        groupBox.Controls.Add(splitContainer);
        resources.ApplyResources(groupBox, "groupBox");
        groupBox.Name = "groupBox";
        groupBox.TabStop = false;
        // 
        // splitContainer
        // 
        resources.ApplyResources(splitContainer, "splitContainer");
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(tableLayoutLeft);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(tableLayoutPanel);
        // 
        // tableLayoutLeft
        // 
        resources.ApplyResources(tableLayoutLeft, "tableLayoutLeft");
        tableLayoutLeft.Controls.Add(groupBoxAvatar, 0, 2);
        tableLayoutLeft.Controls.Add(btnAddPreset, 0, 0);
        tableLayoutLeft.Controls.Add(textBoxAvatarFilter, 0, 1);
        tableLayoutLeft.Name = "tableLayoutLeft";
        // 
        // groupBoxAvatar
        // 
        groupBoxAvatar.Controls.Add(flpAvatars);
        resources.ApplyResources(groupBoxAvatar, "groupBoxAvatar");
        groupBoxAvatar.Name = "groupBoxAvatar";
        groupBoxAvatar.TabStop = false;
        // 
        // flpAvatars
        // 
        resources.ApplyResources(flpAvatars, "flpAvatars");
        flpAvatars.BackColor = SystemColors.ControlLightLight;
        flpAvatars.BorderStyle = BorderStyle.FixedSingle;
        flpAvatars.Name = "flpAvatars";
        // 
        // btnAddPreset
        // 
        resources.ApplyResources(btnAddPreset, "btnAddPreset");
        btnAddPreset.Name = "btnAddPreset";
        btnAddPreset.UseVisualStyleBackColor = true;
        btnAddPreset.Click += btnAddPreset_Click;
        // 
        // textBoxAvatarFilter
        // 
        resources.ApplyResources(textBoxAvatarFilter, "textBoxAvatarFilter");
        textBoxAvatarFilter.Name = "textBoxAvatarFilter";
        textBoxAvatarFilter.TextChanged += textBoxAvatarFilter_TextChanged;
        // 
        // tableLayoutPanel
        // 
        resources.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
        tableLayoutPanel.Controls.Add(groupBoxSelectedAvatar, 0, 2);
        tableLayoutPanel.Controls.Add(labelUid, 0, 0);
        tableLayoutPanel.Controls.Add(labelScene, 0, 1);
        tableLayoutPanel.Controls.Add(cbAutoEquip, 1, 1);
        tableLayoutPanel.Controls.Add(labelConflict, 1, 0);
        tableLayoutPanel.Controls.Add(btnConfirmConflict, 4, 0);
        tableLayoutPanel.Controls.Add(cbEnableGameMessage, 2, 1);
        tableLayoutPanel.Controls.Add(btnRefreshMetadata, 3, 1);
        tableLayoutPanel.Name = "tableLayoutPanel";
        // 
        // groupBoxSelectedAvatar
        // 
        tableLayoutPanel.SetColumnSpan(groupBoxSelectedAvatar, 5);
        groupBoxSelectedAvatar.Controls.Add(flpProfileDetails);
        resources.ApplyResources(groupBoxSelectedAvatar, "groupBoxSelectedAvatar");
        groupBoxSelectedAvatar.Name = "groupBoxSelectedAvatar";
        groupBoxSelectedAvatar.TabStop = false;
        // 
        // flpProfileDetails
        // 
        resources.ApplyResources(flpProfileDetails, "flpProfileDetails");
        flpProfileDetails.BackColor = SystemColors.Window;
        flpProfileDetails.BorderStyle = BorderStyle.FixedSingle;
        flpProfileDetails.Name = "flpProfileDetails";
        // 
        // labelUid
        // 
        resources.ApplyResources(labelUid, "labelUid");
        labelUid.Name = "labelUid";
        // 
        // labelScene
        // 
        resources.ApplyResources(labelScene, "labelScene");
        labelScene.Name = "labelScene";
        // 
        // cbAutoEquip
        // 
        resources.ApplyResources(cbAutoEquip, "cbAutoEquip");
        cbAutoEquip.Name = "cbAutoEquip";
        cbAutoEquip.UseVisualStyleBackColor = true;
        // 
        // labelConflict
        // 
        resources.ApplyResources(labelConflict, "labelConflict");
        tableLayoutPanel.SetColumnSpan(labelConflict, 3);
        labelConflict.ForeColor = Color.DarkRed;
        labelConflict.Name = "labelConflict";
        // 
        // btnConfirmConflict
        // 
        resources.ApplyResources(btnConfirmConflict, "btnConfirmConflict");
        btnConfirmConflict.Name = "btnConfirmConflict";
        btnConfirmConflict.UseVisualStyleBackColor = true;
        btnConfirmConflict.Click += btnConfirmConflict_Click;
        // 
        // cbEnableGameMessage
        // 
        resources.ApplyResources(cbEnableGameMessage, "cbEnableGameMessage");
        cbEnableGameMessage.Name = "cbEnableGameMessage";
        cbEnableGameMessage.UseVisualStyleBackColor = true;
        // 
        // btnRefreshMetadata
        // 
        resources.ApplyResources(btnRefreshMetadata, "btnRefreshMetadata");
        btnRefreshMetadata.Name = "btnRefreshMetadata";
        btnRefreshMetadata.UseVisualStyleBackColor = true;
        // 
        // MainForm
        // 
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(groupBox);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MainMenuStrip = menuStrip;
        MaximizeBox = false;
        Name = "MainForm";
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
