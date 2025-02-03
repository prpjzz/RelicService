using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Service;
using RelicService.Tools;

namespace RelicService.View;

internal class AddPresetForm : Form
{
	private static readonly Size TeamInfoPanelSize = new Size(475, 80);

	private static readonly Size AllInfoPanelSize = new Size(455, 75);

	private readonly AvatarService _avatarService;

	private readonly EquipService _equipService;

	private readonly StatusService _statusService;

	private readonly EventManager _eventManager;

	private readonly ResourceManager _resourceManager;

	private readonly SqliteContext _dbContext;

	private readonly Dictionary<ulong, AvatarRelicInfo> _teamAvatarControlsMap = new Dictionary<ulong, AvatarRelicInfo>();

	private readonly Dictionary<ulong, AvatarRelicInfo> _allAvatarControlsMap = new Dictionary<ulong, AvatarRelicInfo>();

	private ulong _lastClickedTeamAvatar;

	private ulong _lastClickedAllAvatar;

	private readonly Dictionary<ulong, DbUserAvatar> _avatarDataDict = new Dictionary<ulong, DbUserAvatar>();

	private List<DbUserAvatar> _checkedListCache = new List<DbUserAvatar>();

	private List<DbUserAvatar> _currentTeamCache = new List<DbUserAvatar>();

	private readonly Dictionary<uint, Image> _avatarImageCache = new Dictionary<uint, Image>();

	private readonly Dictionary<uint, Image> _relicImageCache = new Dictionary<uint, Image>();

	private readonly HashSet<uint> _avatarImageRef = new HashSet<uint>();

	private readonly HashSet<uint> _relicImageRef = new HashSet<uint>();

	private IContainer components;

	private SplitContainer splitContainer;

	private GroupBox groupBoxRight;

	private TabControl tabControl;

	private TabPage tabCurrentTeam;

	private TabPage tabAllAvatar;

	private TableLayoutPanel tableLayoutPanel;

	private TextBox textBoxProfileName;

	private Label label1;

	private StatusStrip statusStrip;

	private ToolStripStatusLabel statusLabel;

	private GroupBox groupBoxAutoEquip;

	private Button btnSave;

	private TableLayoutPanel tableLayoutAutoEquip;

	private CheckBox cbEnableAutoEquip;

	private CheckBox cbActiveWithScene;

	private Label labelSceneId;

	private TextBox tbSceneIds;

	private CheckBox cbActiveWithTeam;

	private Label labelTeammates;

	private Label labelSelectedTeammates;

	private TextBox tbTeammateSearch;

	private Button btnUseCurrentTeam;

	private FlowLayoutPanel flpCurrentTeam;

	private Button btnRefreshTeam;

	private FlowLayoutPanel flpAllAvatar;

	private Button btnRefreshAllAvatar;

	private FlowLayoutPanel flpAvatars;

	private Button btnClearSelectedTeam;

	public AddPresetForm()
	{
	}

	public AddPresetForm(AvatarService avatarService, EquipService equipService, StatusService statusService, EventManager eventManager, ResourceManager resourceManager, SqliteContext dbContext)
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

	private void AddPresetForm_Load(object sender, EventArgs e)
	{
		SetupBindings();
		tabControl.Enabled = false;
		statusLabel.Text = "Starting...";
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
		_avatarImageRef.ToList().ForEach(_resourceManager.FreeAvatarImage);
		_relicImageRef.ToList().ForEach(_resourceManager.FreeRelicImage);
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
		}
		tabControl.Enabled = true;
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = textBoxProfileName.Text;
			bool @checked = cbEnableAutoEquip.Checked;
			bool checked2 = cbActiveWithScene.Checked;
			string text2 = tbSceneIds.Text;
			bool checked3 = cbActiveWithTeam.Checked;
			await CreateAndSaveConfig(text, @checked, checked2, text2, checked3);
			await _dbContext.SaveChangesAsync();
			MessageBox.Show("Save Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Task.Run(async delegate
			{
				await Task.Delay(800);
				Invoke(delegate
				{
					statusLabel.Text = "Save Success";
				});
				await Task.Delay(2000);
				Invoke(delegate
				{
					statusLabel.Text = "OK";
				});
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
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private async void btnRefreshTeam_Click(object sender, EventArgs e)
	{
		flpCurrentTeam.Controls.OfType<Control>().ToList().ForEach(delegate(Control x)
		{
			x.Dispose();
		});
		flpCurrentTeam.Controls.Clear();
		_teamAvatarControlsMap.Clear();
		tabControl.Enabled = false;
		await OnSwitchToTeamTab();
		tabControl.Enabled = true;
	}

	private async void btnRefreshAllAvatar_Click(object sender, EventArgs e)
	{
		flpAllAvatar.Controls.OfType<Control>().ToList().ForEach(delegate(Control x)
		{
			x.Dispose();
		});
		flpAllAvatar.Controls.Clear();
		_allAvatarControlsMap.Clear();
		tabControl.Enabled = false;
		await OnSwitchToAllAvatarsTab();
		tabControl.Enabled = true;
	}

	private void btnUseCurrentTeam_Click(object sender, EventArgs e)
	{
		List<CheckBox> list = flpAvatars.Controls.OfType<CheckBox>().ToList();
		List<ulong> list2 = (from x in _currentTeamCache
			where x.Guid != _lastClickedTeamAvatar
			select x.Guid).ToList();
		foreach (CheckBox item in list)
		{
			item.Checked = list2.Contains((ulong)item.Tag);
		}
	}

	private void btnClearSelectedTeam_Click(object sender, EventArgs e)
	{
		flpAvatars.Controls.OfType<CheckBox>().ToList().ForEach(delegate(CheckBox x)
		{
			x.Checked = false;
		});
	}

	private void tbTeammateSearch_TextChanged(object sender, EventArgs e)
	{
		List<CheckBox> list = flpAvatars.Controls.OfType<CheckBox>().ToList();
		if (string.IsNullOrWhiteSpace(tbTeammateSearch.Text))
		{
			list.ForEach(delegate(CheckBox x)
			{
				x.Visible = true;
			});
		}
		else
		{
			list.ForEach(delegate(CheckBox x)
			{
				x.Visible = x.Text.Contains(tbTeammateSearch.Text);
			});
		}
	}

	private void flpAvatars_OnCheckedChanged(object? sender, EventArgs e)
	{
		List<CheckBox> list = flpAvatars.Controls.OfType<CheckBox>().ToList();
		int checkedCount = list.Count((CheckBox x) => x.Checked);
		list.ForEach(delegate(CheckBox x)
		{
			x.Enabled = checkedCount < 3 || x.Checked;
		});
		if (checkedCount > 0)
		{
			string text = (from x in list
				where x.Checked
				select x.Text).Aggregate((string x, string y) => x + "," + y);
			labelSelectedTeammates.Text = text;
		}
		else
		{
			labelSelectedTeammates.Text = "Nothing";
		}
	}

	private void SetupBindings()
	{
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
		if (base.InvokeRequired)
		{
			Invoke(delegate
			{
				OnFetechProgressEvent(sender, e);
			});
			return;
		}
		if (e.Type == FetchType.None)
		{
			statusLabel.Text = "OK";
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Loading Character from Account ");
		stringBuilder.Append(e.Name);
		if (e.Total != 0)
		{
			stringBuilder.Append(" (");
			stringBuilder.Append(e.Current);
			stringBuilder.Append("/");
			stringBuilder.Append(e.Total);
			stringBuilder.Append(")");
		}
		statusLabel.Text = stringBuilder.ToString();
	}

	private void OnUidChanged(object? sender, uint uid)
	{
		if (base.InvokeRequired)
		{
			Invoke(delegate
			{
				OnUidChanged(sender, uid);
			});
		}
		else
		{
			Close();
		}
	}

	private async Task PopulateAvatarCheckboxList()
	{
		if (_statusService.CurrentUid == 0)
		{
			return;
		}
		List<DbUserAvatar> list = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
		if (list == null || list.Count <= 4)
		{
			await _avatarService.UpdateAllAvatarFromGame();
			list = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
		}
		if (list == null)
		{
			return;
		}
		list = list.OrderBy((DbUserAvatar x) => x.AvatarId).ToList();
		_checkedListCache = list;
		flpAvatars.Controls.Clear();
		foreach (DbUserAvatar item in _checkedListCache)
		{
			CheckBox checkBox = new CheckBox();
			checkBox.Text = item.Avatar.Name;
			checkBox.Tag = item.Guid;
			checkBox.Margin = new Padding(3, 3, 3, 3);
			checkBox.CheckedChanged += flpAvatars_OnCheckedChanged;
			flpAvatars.Controls.Add(checkBox);
			_avatarDataDict.Add(item.Guid, item);
		}
	}

	private async Task OnSwitchToTeamTab()
	{
		btnUseCurrentTeam.Visible = true;
		groupBoxRight.Enabled = _lastClickedTeamAvatar != 0;
		if (flpCurrentTeam.Controls.Count > 0)
		{
			return;
		}
		_lastClickedTeamAvatar = 0uL;
		await _avatarService.UpdateTeamFromGame();
		List<DbUserAvatar> currentTeam = await _avatarService.GetCurrentTeam();
		if (currentTeam == null)
		{
			statusLabel.Text = "Current team information not found";
			return;
		}
		List<ulong> list = currentTeam.Select((DbUserAvatar x) => x.Guid).ToList();
		Dictionary<ulong, List<RelicDataDto>> equipDict = new Dictionary<ulong, List<RelicDataDto>>();
		foreach (ulong avatarGuid in list)
		{
			List<RelicDataDto> list2 = await _equipService.UpdateAvatarEquipFromGame(avatarGuid);
			if (list2 != null)
			{
				equipDict.Add(avatarGuid, list2);
			}
		}
		foreach (DbUserAvatar avatar in currentTeam)
		{
			AvatarRelicInfo avatarRelicInfo = await CreateInfoPanel(avatar, equipDict);
			if (avatarRelicInfo != null)
			{
				avatarRelicInfo.Size = TeamInfoPanelSize;
				avatarRelicInfo.OnClickCallback = OnTeamAvatarEntryClick;
				int num = flpCurrentTeam.ClientSize.Width - flpCurrentTeam.Margin.Horizontal;
				avatarRelicInfo.MinimumSize = new Size(num, (int)((double)num * 0.16666));
				_teamAvatarControlsMap.Add(avatar.Guid, avatarRelicInfo);
				flpCurrentTeam.Controls.Add(avatarRelicInfo);
			}
		}
		_currentTeamCache = currentTeam;
	}

	private async Task OnSwitchToAllAvatarsTab()
	{
		btnUseCurrentTeam.Visible = false;
		groupBoxRight.Enabled = _lastClickedAllAvatar != 0;
		if (flpAllAvatar.Controls.Count > 0 || _statusService.CurrentUid == 0)
		{
			return;
		}
		_lastClickedAllAvatar = 0uL;
		await _avatarService.UpdateAllAvatarFromGame();
		List<DbUserAvatar> avatars = await _avatarService.GetUserAvatars(_statusService.CurrentUid);
		if (avatars == null)
		{
			statusLabel.Text = "Current user profile information not found";
			return;
		}
		avatars = avatars.OrderBy((DbUserAvatar x) => x.AvatarId).ToList();
		List<ulong> avatarGuidList = avatars.Select((DbUserAvatar x) => x.Guid).ToList();
		_eventManager.OnFetchProgress -= OnFetechProgressEvent;
		new List<Task<(ulong, List<RelicDataDto>)>>();
		Dictionary<ulong, List<RelicDataDto>> equipDict = new Dictionary<ulong, List<RelicDataDto>>();
		int processedCount = 1;
		foreach (ulong avatarGuid in avatarGuidList)
		{
			statusLabel.Text = $"Getting personal equipment information ({processedCount}/{avatarGuidList.Count})";
			List<RelicDataDto> list = await _equipService.UpdateAvatarEquipFromGame(avatarGuid);
			if (list != null)
			{
				equipDict.Add(avatarGuid, list);
			}
			processedCount++;
		}
		foreach (DbUserAvatar avatar in avatars)
		{
			AvatarRelicInfo avatarRelicInfo = await CreateInfoPanel(avatar, equipDict);
			if (avatarRelicInfo != null)
			{
				avatarRelicInfo.Size = AllInfoPanelSize;
				avatarRelicInfo.OnClickCallback = OnAllAvatarEntryClick;
				int num = flpAllAvatar.Width - SystemInformation.VerticalScrollBarWidth - flpAllAvatar.Margin.Horizontal;
				avatarRelicInfo.MinimumSize = new Size(num, (int)((double)num * 0.16666));
				_allAvatarControlsMap.Add(avatar.Guid, avatarRelicInfo);
				flpAllAvatar.Controls.Add(avatarRelicInfo);
			}
		}
		statusLabel.Text = "OK";
		_eventManager.OnFetchProgress += OnFetechProgressEvent;
	}

	private async Task<AvatarRelicInfo?> CreateInfoPanel(DbUserAvatar avatar, Dictionary<ulong, List<RelicDataDto>> equipDict)
	{
		if (!_avatarImageCache.TryGetValue(avatar.AvatarId, out Image value))
		{
			value = await _resourceManager.GetAvatarImage(avatar.AvatarId);
			if (value == null)
			{
				return null;
			}
			value = Utils.ResizeImage(value, 70, 70);
			_avatarImageRef.Add(avatar.AvatarId);
			_avatarImageCache.Add(avatar.AvatarId, value);
		}
		AvatarRelicInfo panel = new AvatarRelicInfo(avatar.Guid)
		{
			AvatarImage = value,
			Dock = DockStyle.Top
		};
		foreach (RelicDataDto relic in equipDict[avatar.Guid])
		{
			if (!_relicImageCache.TryGetValue(relic.ItemId, out Image value2))
			{
				value2 = await _resourceManager.GetRelicImage(relic.ItemId);
				if (value2 == null)
				{
					continue;
				}
				value2 = Utils.ResizeImage(value2, 70, 70);
				_relicImageRef.Add(relic.ItemId);
				_relicImageCache.Add(relic.ItemId, value2);
			}
			string propStatTooltip = GetPropStatTooltip(relic);
			switch (relic.EquipType)
			{
			case EquipType.EQUIP_BRACER:
				panel.FlowerImage = value2;
				panel.FlowerTooltip = propStatTooltip;
				break;
			case EquipType.EQUIP_NECKLACE:
				panel.FeatherImage = value2;
				panel.FeatherTooltip = propStatTooltip;
				break;
			case EquipType.EQUIP_SHOES:
				panel.SandImage = value2;
				panel.SandTooltip = propStatTooltip;
				break;
			case EquipType.EQUIP_RING:
				panel.GobletImage = value2;
				panel.GobletTooltip = propStatTooltip;
				break;
			case EquipType.EQUIP_DRESS:
				panel.HeadImage = value2;
				panel.HeadTooltip = propStatTooltip;
				break;
			}
		}
		return panel;
	}

	private void OnTeamAvatarEntryClick(ulong avatarGuid)
	{
		if (_lastClickedTeamAvatar != 0L)
		{
			_teamAvatarControlsMap[_lastClickedTeamAvatar].IsSelected = false;
		}
		_teamAvatarControlsMap[avatarGuid].IsSelected = true;
		_lastClickedTeamAvatar = avatarGuid;
		groupBoxRight.Enabled = true;
	}

	private void OnAllAvatarEntryClick(ulong avatarGuid)
	{
		if (_lastClickedAllAvatar != 0L)
		{
			_allAvatarControlsMap[_lastClickedAllAvatar].IsSelected = false;
		}
		_allAvatarControlsMap[avatarGuid].IsSelected = true;
		_lastClickedAllAvatar = avatarGuid;
		groupBoxRight.Enabled = true;
	}

	private string GetPropStatTooltip(RelicDataDto relic)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		stringBuilder.Append(relic.Level - 1);
		stringBuilder.Append("Level] ");
		string value = Utils.FormatFightProp(relic.MainPropType, relic.MainPropValue);
		stringBuilder.Append(value);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		foreach (RelicAffixDto item in relic.AppendProp)
		{
			string value2 = Utils.FormatFightProp(item.PropType, item.PropValue);
			stringBuilder.Append(value2);
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	private async Task CreateAndSaveConfig(string profileName, bool autoEquip, bool activeWithScene, string sceneIds, bool activeWithTeam)
	{
		if (string.IsNullOrWhiteSpace(profileName))
		{
			throw new Exception("Default name cannot be empty");
		}
		ulong selectedAvatarGuid = ((tabControl.SelectedIndex == 0) ? _lastClickedTeamAvatar : _lastClickedAllAvatar);
		DbUserAvatar selectedAvatar = _avatarDataDict[selectedAvatarGuid];
		if (await _dbContext.RelicProfiles.Where((DbRelicProfile x) => x.ProfileName == profileName && x.AvatarGuid == selectedAvatarGuid).FirstOrDefaultAsync() != null)
		{
			throw new Exception("[" + selectedAvatar.Avatar.Name + "] Preset name already exists");
		}
		List<DbRelicItem> list = await _equipService.UpdateAndSaveAvatarEquip(selectedAvatarGuid);
		if (list.Count == 0)
		{
			throw new Exception("不能创建无圣遗物的预设");
		}
		DbRelicProfile dbRelicProfile = new DbRelicProfile();
		dbRelicProfile.ProfileName = profileName;
		dbRelicProfile.UserAvatar = selectedAvatar;
		dbRelicProfile.RelicItems = list;
		if (autoEquip)
		{
			if (activeWithScene)
			{
				string[] array = sceneIds.Replace(" ", "").Split(',');
				if (array.Length == 0)
				{
					throw new Exception("Scene ID cannot be empty");
				}
				try
				{
					List<uint> withScene = array.Select(uint.Parse).ToList();
					dbRelicProfile.WithScene = withScene;
				}
				catch (Exception)
				{
					throw new Exception("The scene ID must be a number.");
				}
			}
			if (activeWithTeam)
			{
				List<ulong> list2 = (from x in flpAvatars.Controls.OfType<CheckBox>().ToList()
					where x.Checked
					select (ulong)x.Tag).ToList();
				if (list2.Count > 3)
				{
					throw new Exception("The number of teams cannot exceed three.");
				}
				if (list2.Count == 0)
				{
					throw new Exception("The number of teams cannot be 0.");
				}
				List<DbUserAvatar> source = (from x in list2.Where(_avatarDataDict.ContainsKey)
					select _avatarDataDict[x]).ToList();
				DbRelicProfileTeamContext dbRelicProfileTeamContext = new DbRelicProfileTeamContext();
				dbRelicProfileTeamContext.AvatarIds = source.Select((DbUserAvatar x) => x.AvatarId).ToList();
				dbRelicProfileTeamContext.Profile = dbRelicProfile;
				_dbContext.RelicProfileTeamContext.Add(dbRelicProfileTeamContext);
			}
			if (activeWithScene || activeWithTeam)
			{
				dbRelicProfile.AutoEquip = true;
			}
		}
		_dbContext.RelicProfiles.Add(dbRelicProfile);
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
        ComponentResourceManager resources = new ComponentResourceManager(typeof(AddPresetForm));
        splitContainer = new SplitContainer();
        tabControl = new TabControl();
        tabCurrentTeam = new TabPage();
        btnRefreshTeam = new Button();
        flpCurrentTeam = new FlowLayoutPanel();
        tabAllAvatar = new TabPage();
        btnRefreshAllAvatar = new Button();
        flpAllAvatar = new FlowLayoutPanel();
        groupBoxRight = new GroupBox();
        tableLayoutPanel = new TableLayoutPanel();
        textBoxProfileName = new TextBox();
        label1 = new Label();
        groupBoxAutoEquip = new GroupBox();
        tableLayoutAutoEquip = new TableLayoutPanel();
        cbEnableAutoEquip = new CheckBox();
        cbActiveWithScene = new CheckBox();
        labelSceneId = new Label();
        tbSceneIds = new TextBox();
        cbActiveWithTeam = new CheckBox();
        labelTeammates = new Label();
        labelSelectedTeammates = new Label();
        tbTeammateSearch = new TextBox();
        btnUseCurrentTeam = new Button();
        flpAvatars = new FlowLayoutPanel();
        btnClearSelectedTeam = new Button();
        btnSave = new Button();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        ((ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        tabControl.SuspendLayout();
        tabCurrentTeam.SuspendLayout();
        tabAllAvatar.SuspendLayout();
        groupBoxRight.SuspendLayout();
        tableLayoutPanel.SuspendLayout();
        groupBoxAutoEquip.SuspendLayout();
        tableLayoutAutoEquip.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer
        // 
        splitContainer.IsSplitterFixed = true;
        splitContainer.Location = new Point(0, 3);
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(tabControl);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(groupBoxRight);
        splitContainer.Size = new Size(800, 422);
        splitContainer.SplitterDistance = 500;
        splitContainer.TabIndex = 0;
        // 
        // tabControl
        // 
        tabControl.Controls.Add(tabCurrentTeam);
        tabControl.Controls.Add(tabAllAvatar);
        tabControl.Location = new Point(5, 0);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(495, 419);
        tabControl.TabIndex = 0;
        tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
        // 
        // tabCurrentTeam
        // 
        tabCurrentTeam.Controls.Add(btnRefreshTeam);
        tabCurrentTeam.Controls.Add(flpCurrentTeam);
        tabCurrentTeam.Location = new Point(4, 24);
        tabCurrentTeam.Name = "tabCurrentTeam";
        tabCurrentTeam.Padding = new Padding(3);
        tabCurrentTeam.Size = new Size(487, 391);
        tabCurrentTeam.TabIndex = 0;
        tabCurrentTeam.Text = "Current Team";
        tabCurrentTeam.UseVisualStyleBackColor = true;
        // 
        // btnRefreshTeam
        // 
        btnRefreshTeam.Dock = DockStyle.Bottom;
        btnRefreshTeam.Location = new Point(3, 365);
        btnRefreshTeam.Name = "btnRefreshTeam";
        btnRefreshTeam.Size = new Size(481, 23);
        btnRefreshTeam.TabIndex = 1;
        btnRefreshTeam.Text = "Refresh List";
        btnRefreshTeam.UseVisualStyleBackColor = true;
        btnRefreshTeam.Click += btnRefreshTeam_Click;
        // 
        // flpCurrentTeam
        // 
        flpCurrentTeam.AutoScroll = true;
        flpCurrentTeam.Dock = DockStyle.Top;
        flpCurrentTeam.FlowDirection = FlowDirection.TopDown;
        flpCurrentTeam.Location = new Point(3, 3);
        flpCurrentTeam.Name = "flpCurrentTeam";
        flpCurrentTeam.Size = new Size(481, 360);
        flpCurrentTeam.TabIndex = 0;
        flpCurrentTeam.WrapContents = false;
        // 
        // tabAllAvatar
        // 
        tabAllAvatar.Controls.Add(btnRefreshAllAvatar);
        tabAllAvatar.Controls.Add(flpAllAvatar);
        tabAllAvatar.Location = new Point(4, 24);
        tabAllAvatar.Name = "tabAllAvatar";
        tabAllAvatar.Padding = new Padding(3);
        tabAllAvatar.Size = new Size(487, 391);
        tabAllAvatar.TabIndex = 1;
        tabAllAvatar.Text = "All Character";
        tabAllAvatar.UseVisualStyleBackColor = true;
        // 
        // btnRefreshAllAvatar
        // 
        btnRefreshAllAvatar.Dock = DockStyle.Bottom;
        btnRefreshAllAvatar.Location = new Point(3, 365);
        btnRefreshAllAvatar.Name = "btnRefreshAllAvatar";
        btnRefreshAllAvatar.Size = new Size(481, 23);
        btnRefreshAllAvatar.TabIndex = 1;
        btnRefreshAllAvatar.Text = "Refresh List";
        btnRefreshAllAvatar.UseVisualStyleBackColor = true;
        btnRefreshAllAvatar.Click += btnRefreshAllAvatar_Click;
        // 
        // flpAllAvatar
        // 
        flpAllAvatar.AutoScroll = true;
        flpAllAvatar.Dock = DockStyle.Top;
        flpAllAvatar.FlowDirection = FlowDirection.TopDown;
        flpAllAvatar.Location = new Point(3, 3);
        flpAllAvatar.Name = "flpAllAvatar";
        flpAllAvatar.Size = new Size(481, 360);
        flpAllAvatar.TabIndex = 0;
        flpAllAvatar.WrapContents = false;
        // 
        // groupBoxRight
        // 
        groupBoxRight.Controls.Add(tableLayoutPanel);
        groupBoxRight.Location = new Point(0, 0);
        groupBoxRight.Name = "groupBoxRight";
        groupBoxRight.Size = new Size(290, 419);
        groupBoxRight.TabIndex = 0;
        groupBoxRight.TabStop = false;
        groupBoxRight.Text = "Pre-setting";
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.ColumnCount = 2;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(textBoxProfileName, 1, 0);
        tableLayoutPanel.Controls.Add(label1, 0, 0);
        tableLayoutPanel.Controls.Add(groupBoxAutoEquip, 0, 1);
        tableLayoutPanel.Controls.Add(btnSave, 0, 2);
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.Location = new Point(3, 19);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(284, 397);
        tableLayoutPanel.TabIndex = 0;
        // 
        // textBoxProfileName
        // 
        textBoxProfileName.Dock = DockStyle.Fill;
        textBoxProfileName.Location = new Point(96, 3);
        textBoxProfileName.Name = "textBoxProfileName";
        textBoxProfileName.Size = new Size(185, 23);
        textBoxProfileName.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(7, 7);
        label1.Margin = new Padding(7);
        label1.Name = "label1";
        label1.Size = new Size(79, 15);
        label1.TabIndex = 1;
        label1.Text = "Pre-set Name";
        // 
        // groupBoxAutoEquip
        // 
        tableLayoutPanel.SetColumnSpan(groupBoxAutoEquip, 2);
        groupBoxAutoEquip.Controls.Add(tableLayoutAutoEquip);
        groupBoxAutoEquip.Dock = DockStyle.Fill;
        groupBoxAutoEquip.Location = new Point(10, 39);
        groupBoxAutoEquip.Margin = new Padding(10);
        groupBoxAutoEquip.Name = "groupBoxAutoEquip";
        groupBoxAutoEquip.Size = new Size(264, 319);
        groupBoxAutoEquip.TabIndex = 2;
        groupBoxAutoEquip.TabStop = false;
        groupBoxAutoEquip.Text = "Auto Equip (Optional)";
        // 
        // tableLayoutAutoEquip
        // 
        tableLayoutAutoEquip.ColumnCount = 2;
        tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle());
        tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutAutoEquip.Controls.Add(cbEnableAutoEquip, 0, 0);
        tableLayoutAutoEquip.Controls.Add(cbActiveWithScene, 0, 1);
        tableLayoutAutoEquip.Controls.Add(labelSceneId, 0, 2);
        tableLayoutAutoEquip.Controls.Add(tbSceneIds, 1, 2);
        tableLayoutAutoEquip.Controls.Add(cbActiveWithTeam, 0, 3);
        tableLayoutAutoEquip.Controls.Add(labelTeammates, 0, 4);
        tableLayoutAutoEquip.Controls.Add(labelSelectedTeammates, 1, 4);
        tableLayoutAutoEquip.Controls.Add(tbTeammateSearch, 1, 5);
        tableLayoutAutoEquip.Controls.Add(btnUseCurrentTeam, 0, 5);
        tableLayoutAutoEquip.Controls.Add(flpAvatars, 1, 6);
        tableLayoutAutoEquip.Controls.Add(btnClearSelectedTeam, 0, 6);
        tableLayoutAutoEquip.Dock = DockStyle.Fill;
        tableLayoutAutoEquip.Location = new Point(3, 19);
        tableLayoutAutoEquip.Name = "tableLayoutAutoEquip";
        tableLayoutAutoEquip.RowCount = 7;
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
        tableLayoutAutoEquip.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutAutoEquip.Size = new Size(258, 297);
        tableLayoutAutoEquip.TabIndex = 0;
        // 
        // cbEnableAutoEquip
        // 
        cbEnableAutoEquip.AutoSize = true;
        cbEnableAutoEquip.Location = new Point(7, 4);
        cbEnableAutoEquip.Margin = new Padding(7, 4, 7, 4);
        cbEnableAutoEquip.Name = "cbEnableAutoEquip";
        cbEnableAutoEquip.Size = new Size(59, 19);
        cbEnableAutoEquip.TabIndex = 0;
        cbEnableAutoEquip.Text = "Active";
        cbEnableAutoEquip.UseVisualStyleBackColor = true;
        // 
        // cbActiveWithScene
        // 
        cbActiveWithScene.AutoSize = true;
        cbActiveWithScene.Location = new Point(7, 31);
        cbActiveWithScene.Margin = new Padding(7, 4, 5, 4);
        cbActiveWithScene.Name = "cbActiveWithScene";
        cbActiveWithScene.Size = new Size(120, 19);
        cbActiveWithScene.TabIndex = 1;
        cbActiveWithScene.Text = "Active With scene";
        cbActiveWithScene.UseVisualStyleBackColor = true;
        // 
        // labelSceneId
        // 
        labelSceneId.AutoSize = true;
        labelSceneId.Location = new Point(7, 61);
        labelSceneId.Margin = new Padding(7);
        labelSceneId.Name = "labelSceneId";
        labelSceneId.Size = new Size(52, 15);
        labelSceneId.TabIndex = 2;
        labelSceneId.Text = "Scene ID";
        // 
        // tbSceneIds
        // 
        tbSceneIds.Dock = DockStyle.Fill;
        tbSceneIds.Location = new Point(135, 57);
        tbSceneIds.Name = "tbSceneIds";
        tbSceneIds.PlaceholderText = "Example: 3,1016,1056";
        tbSceneIds.Size = new Size(120, 23);
        tbSceneIds.TabIndex = 3;
        // 
        // cbActiveWithTeam
        // 
        cbActiveWithTeam.AutoSize = true;
        cbActiveWithTeam.Location = new Point(7, 87);
        cbActiveWithTeam.Margin = new Padding(7, 4, 5, 4);
        cbActiveWithTeam.Name = "cbActiveWithTeam";
        cbActiveWithTeam.Size = new Size(118, 19);
        cbActiveWithTeam.TabIndex = 4;
        cbActiveWithTeam.Text = "Active With Team";
        cbActiveWithTeam.UseVisualStyleBackColor = true;
        // 
        // labelTeammates
        // 
        labelTeammates.AutoSize = true;
        labelTeammates.Location = new Point(7, 117);
        labelTeammates.Margin = new Padding(7);
        labelTeammates.Name = "labelTeammates";
        labelTeammates.Size = new Size(62, 15);
        labelTeammates.TabIndex = 5;
        labelTeammates.Text = "Teammate";
        // 
        // labelSelectedTeammates
        // 
        labelSelectedTeammates.AutoSize = true;
        labelSelectedTeammates.Location = new Point(132, 117);
        labelSelectedTeammates.Margin = new Padding(0, 7, 7, 7);
        labelSelectedTeammates.Name = "labelSelectedTeammates";
        labelSelectedTeammates.Size = new Size(51, 15);
        labelSelectedTeammates.TabIndex = 6;
        labelSelectedTeammates.Text = "Nothing";
        // 
        // tbTeammateSearch
        // 
        tbTeammateSearch.Dock = DockStyle.Fill;
        tbTeammateSearch.Location = new Point(135, 142);
        tbTeammateSearch.Name = "tbTeammateSearch";
        tbTeammateSearch.PlaceholderText = "Search for";
        tbTeammateSearch.Size = new Size(120, 23);
        tbTeammateSearch.TabIndex = 9;
        tbTeammateSearch.TextChanged += tbTeammateSearch_TextChanged;
        // 
        // btnUseCurrentTeam
        // 
        btnUseCurrentTeam.Dock = DockStyle.Fill;
        btnUseCurrentTeam.Location = new Point(3, 142);
        btnUseCurrentTeam.Name = "btnUseCurrentTeam";
        btnUseCurrentTeam.Size = new Size(126, 23);
        btnUseCurrentTeam.TabIndex = 10;
        btnUseCurrentTeam.Text = "Use Current Team";
        btnUseCurrentTeam.UseVisualStyleBackColor = true;
        btnUseCurrentTeam.Click += btnUseCurrentTeam_Click;
        // 
        // flpAvatars
        // 
        flpAvatars.AutoScroll = true;
        flpAvatars.BackColor = SystemColors.Window;
        flpAvatars.BorderStyle = BorderStyle.FixedSingle;
        flpAvatars.Dock = DockStyle.Fill;
        flpAvatars.FlowDirection = FlowDirection.TopDown;
        flpAvatars.Location = new Point(135, 171);
        flpAvatars.Name = "flpAvatars";
        flpAvatars.Size = new Size(120, 123);
        flpAvatars.TabIndex = 11;
        flpAvatars.WrapContents = false;
        // 
        // btnClearSelectedTeam
        // 
        btnClearSelectedTeam.Dock = DockStyle.Top;
        btnClearSelectedTeam.Location = new Point(3, 171);
        btnClearSelectedTeam.Name = "btnClearSelectedTeam";
        btnClearSelectedTeam.Size = new Size(126, 23);
        btnClearSelectedTeam.TabIndex = 12;
        btnClearSelectedTeam.Text = "Clear Selection";
        btnClearSelectedTeam.UseVisualStyleBackColor = true;
        btnClearSelectedTeam.Click += btnClearSelectedTeam_Click;
        // 
        // btnSave
        // 
        tableLayoutPanel.SetColumnSpan(btnSave, 2);
        btnSave.Dock = DockStyle.Fill;
        btnSave.Location = new Point(3, 371);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(278, 23);
        btnSave.TabIndex = 3;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 428);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(800, 22);
        statusStrip.SizingGrip = false;
        statusStrip.TabIndex = 1;
        statusStrip.Text = "statusStrip1";
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(55, 17);
        statusLabel.Text = "<Status>";
        // 
        // AddPresetForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(statusStrip);
        Controls.Add(splitContainer);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AddPresetForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add Pre-set";
        FormClosed += AddPresetForm_FormClosed;
        Load += AddPresetForm_Load;
        Shown += AddPresetForm_Shown;
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        tabControl.ResumeLayout(false);
        tabCurrentTeam.ResumeLayout(false);
        tabAllAvatar.ResumeLayout(false);
        groupBoxRight.ResumeLayout(false);
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        groupBoxAutoEquip.ResumeLayout(false);
        tableLayoutAutoEquip.ResumeLayout(false);
        tableLayoutAutoEquip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
