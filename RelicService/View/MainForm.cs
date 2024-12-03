// Decompiled with JetBrains decompiler
// Type: RelicService.View.MainForm
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RelicService.Data;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Event;
using RelicService.Properties;
using RelicService.Service;
using RelicService.Tools;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable enable
namespace RelicService.View
{
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
    private 
    #nullable disable
    IContainer components;
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

    public MainForm(
      #nullable enable
      EventManager eventManager,
      ResourceManager resourceManager,
      StatusService statusService,
      AutoEquipService autoEquipService,
      GameMessageService gameMessageService,
      Network network,
      ApiService apiService,
      SqliteContext dbContext)
    {
      this._eventManager = eventManager;
      this._resourceManager = resourceManager;
      this._statusService = statusService;
      this._autoEquipService = autoEquipService;
      this._gameMessageService = gameMessageService;
      this._network = network;
      this._apiService = apiService;
      this._dbContext = dbContext;
      this.InitializeComponent();
      this._eventManager.OnServiceStatusChanged += new EventHandler<bool>(this.OnServiceStatusChanged);
      this._eventManager.OnUidChanged += new EventHandler<uint>(this.OnUidChanged);
      this._eventManager.OnSceneIdChanged += new EventHandler<uint>(this.OnSceneChanged);
      this._eventManager.OnProfileRefresh += new EventHandler(this.OnProfileRefresh);
      this._eventManager.OnProfileConflict += new EventHandler<string>(this.OnProfileConflict);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.SetupBindings();
      this.labelConflict.Text = string.Empty;
      this.btnConfirmConflict.Visible = false;
      using (Graphics graphics = this.CreateGraphics())
      {
        Program.DpiScaleFactor = graphics.DpiX / 96f;
        Task.Run(new Func<Task>(this.CheckVersion));
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      this._eventManager.FireEventAsync(EventId.EvtShutdown);
    }

    private async void btnAddPreset_Click(object sender, EventArgs e)
    {
      using (AddPresetForm form = Program.ServiceProvider.GetRequiredService<AddPresetForm>())
      {
        int num = (int) form.ShowDialog();
        await this.PopulateAvatarList();
        await this.ChangeRelicDetailDisplay();
      }
    }

    private void btnConfirmConflict_Click(object sender, EventArgs e)
    {
      this.labelConflict.Text = string.Empty;
      this.btnConfirmConflict.Visible = false;
    }

    private void menuAbout_Click(object sender, EventArgs e)
    {
      int num = (int) Program.ServiceProvider.GetRequiredService<AboutForm>().ShowDialog();
    }

    private void textBoxAvatarFilter_TextChanged(object sender, EventArgs e)
    {
      string text = this.textBoxAvatarFilter.Text;
      foreach (object control in (ArrangedElementCollection) this.flpAvatars.Controls)
      {
        if (control is AvatarProfileItem avatarProfileItem)
          avatarProfileItem.Visible = avatarProfileItem.AvatarName.Contains(text) || string.IsNullOrWhiteSpace(text);
      }
    }

    private async Task CheckVersion()
    {
      MainForm mainForm = this;
      try
      {
        string versionInfo1 = await mainForm._network.GetVersionInfo();
        if (versionInfo1 == null)
          return;
        VersionInfo versionInfo2 = JsonConvert.DeserializeObject<VersionInfo>(versionInfo1);
        if (versionInfo2 == null || versionInfo2.Build >= Program.Build)
          return;
        mainForm.Invoke((Action) (() =>
        {
          if (MessageBox.Show("有新版本可用，是否前往下载？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
            return;
          Process.Start(new ProcessStartInfo()
          {
            FileName = "https://space.bilibili.com/44434084/dynamic",
            UseShellExecute = true
          });
        }));
      }
      catch (Exception ex)
      {
      }
    }

    private void SetupBindings()
    {
      this.btnAddPreset.Enabled = this._isOnline;
      this.cbAutoEquip.DataBindings.Add("Checked", (object) this._autoEquipService, "Enabled", false, DataSourceUpdateMode.OnPropertyChanged);
      this.cbEnableGameMessage.DataBindings.Add("Checked", (object) this._gameMessageService, "Enabled", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private async Task PopulateAvatarList()
    {
      MainForm mainForm = this;
      mainForm._avatarProfileItemMap.Clear();
      mainForm._lastSelectedAvatarGuid = 0UL;
      mainForm.ClearAvatarList();
      // ISSUE: reference to a compiler-generated method
      await Task.Run(new Func<Task>(mainForm.\u003CPopulateAvatarList\u003Eb__28_0));
      IIncludableQueryable<DbRelicProfile, DbAvatar> includableQueryable = ((IQueryable<DbRelicProfile>) mainForm._dbContext.RelicProfiles).Include<DbRelicProfile, DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).ThenInclude<DbRelicProfile, DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar));
      Expression<Func<DbRelicProfile, bool>> expression = (Expression<Func<DbRelicProfile, bool>>) (p => p.UserAvatar.UserUid == mainForm._statusService.CurrentUid);
      foreach (DbUserAvatar userAvatar in Enumerable.ToList<DbUserAvatar>((IEnumerable<DbUserAvatar>) Enumerable.OrderBy<DbUserAvatar, uint>(Enumerable.Distinct<DbUserAvatar>(Enumerable.Select<DbRelicProfile, DbUserAvatar>((IEnumerable<DbRelicProfile>) await Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) includableQueryable, expression).ToListAsync<DbRelicProfile>(), (Func<DbRelicProfile, DbUserAvatar>) (p => p.UserAvatar))), (Func<DbUserAvatar, uint>) (a => a.AvatarId))))
      {
        Image image;
        if (!mainForm._avatarImageCache.TryGetValue(userAvatar.AvatarId, ref image))
        {
          image = await mainForm._resourceManager.GetAvatarImage(userAvatar.AvatarId);
          if (image != null)
          {
            image = Utils.ResizeImage(image, 65, 65);
            mainForm._avatarImageCache.Add(userAvatar.AvatarId, image);
          }
          else
            continue;
        }
        AvatarProfileItem avatarProfileItem = new AvatarProfileItem(userAvatar.Guid);
        avatarProfileItem.AvatarImage = image;
        avatarProfileItem.AvatarName = userAvatar.Avatar.Name;
        avatarProfileItem.OnClickCallback = new Action<ulong>(mainForm.OnAvatarProfileItemClicked);
        mainForm._avatarProfileItemMap.TryAdd(userAvatar.Guid, avatarProfileItem);
        mainForm.flpAvatars.Controls.Add((Control) avatarProfileItem);
      }
    }

    private async void OnAvatarProfileItemClicked(ulong avatarGuid)
    {
      if (this._lastSelectedAvatarGuid != 0UL)
        this._avatarProfileItemMap[this._lastSelectedAvatarGuid].IsSelected = false;
      this._avatarProfileItemMap[avatarGuid].IsSelected = true;
      this._lastSelectedAvatarGuid = avatarGuid;
      await this.ChangeRelicDetailDisplay();
    }

    private async Task ChangeRelicDetailDisplay()
    {
      MainForm mainForm = this;
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) mainForm.flpProfileDetails.Controls)).ForEach((Action<Control>) (c => c.Dispose()));
      mainForm.flpProfileDetails.Controls.Clear();
      Enumerable.ToList<uint>((IEnumerable<uint>) mainForm._relicImageRef).ForEach(new Action<uint>(mainForm._resourceManager.FreeRelicImage));
      mainForm._relicImageRef.Clear();
      List<DbRelicProfile> listAsync = await Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) ((IIncludableQueryable<DbRelicProfile, IEnumerable<DbRelicItem>>) ((IQueryable<DbRelicProfile>) ((IIncludableQueryable<DbRelicProfile, IEnumerable<DbRelicItem>>) ((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) mainForm._dbContext.RelicProfiles).Include<DbRelicProfile, DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).ThenInclude<DbRelicProfile, DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar))).Include<DbRelicProfile, ICollection<DbRelicItem>>((Expression<Func<DbRelicProfile, ICollection<DbRelicItem>>>) (p => p.RelicItems))).ThenInclude<DbRelicProfile, DbRelicItem, DbRelic>((Expression<Func<DbRelicItem, DbRelic>>) (r => r.Relic))).Include<DbRelicProfile, ICollection<DbRelicItem>>((Expression<Func<DbRelicProfile, ICollection<DbRelicItem>>>) (p => p.RelicItems))).ThenInclude<DbRelicProfile, DbRelicItem, ICollection<DbRelicAffix>>((Expression<Func<DbRelicItem, ICollection<DbRelicAffix>>>) (r => r.Affixes))).Include<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>((Expression<Func<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>>) (p => p.TeamContexts)), (Expression<Func<DbRelicProfile, bool>>) (p => p.UserAvatar.Guid == mainForm._lastSelectedAvatarGuid)).ToListAsync<DbRelicProfile>();
      if (listAsync.Count == 0)
        return;
      DbUserAvatar userAvatar = Enumerable.First<DbRelicProfile>((IEnumerable<DbRelicProfile>) listAsync).UserAvatar;
      mainForm.groupBoxSelectedAvatar.Text = userAvatar.Avatar.Name;
      foreach (DbRelicProfile profile in listAsync)
      {
        ProfileDetails profileDetailPanel = await mainForm.CreateProfileDetailPanel(profile);
        mainForm.flpProfileDetails.Controls.Add((Control) profileDetailPanel);
      }
      mainForm.CheckProfileDetailSize();
    }

    private async Task<ProfileDetails> CreateProfileDetailPanel(DbRelicProfile profile)
    {
      ProfileDetails panel = Program.ServiceProvider.GetRequiredService<ProfileDetails>();
      panel.ProfileName = profile.ProfileName;
      panel.RelicProfile = profile;
      panel.Dock = DockStyle.Top;
      int height = this.flpProfileDetails.ClientSize.Height;
      int width = this.flpProfileDetails.Width;
      Padding margin = this.flpProfileDetails.Margin;
      int horizontal = ((Padding) ref margin).Horizontal;
      int num = width - horizontal;
      panel.MinimumSize = new Size(num, (int) ((double) num * (double) ProfileDetails.WidthToHeightScale));
      foreach (DbRelicItem relic in (IEnumerable<DbRelicItem>) profile.RelicItems)
      {
        Image image;
        if (!this._relicImageCache.TryGetValue(relic.ItemId, ref image))
        {
          image = await this._resourceManager.GetRelicImage(relic.ItemId);
          if (image != null)
          {
            image = Utils.ResizeImage(image, 80, 80);
            this._relicImageCache.Add(relic.ItemId, image);
            this._relicImageRef.Add(relic.ItemId);
          }
          else
            continue;
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Utils.FormatFightPropShort(relic.MainPropType, relic.MainPropValue));
        stringBuilder.AppendLine();
        foreach (DbRelicAffix affix in (IEnumerable<DbRelicAffix>) relic.Affixes)
        {
          string str = Utils.FormatFightPropShort(affix.PropType, affix.PropValue);
          stringBuilder.AppendLine(str);
        }
        switch (relic.EquipType)
        {
          case EquipType.EQUIP_BRACER:
            panel.FlowerImage = image;
            panel.FlowerLabel = stringBuilder.ToString();
            break;
          case EquipType.EQUIP_NECKLACE:
            panel.FeatherImage = image;
            panel.FeatherLabel = stringBuilder.ToString();
            break;
          case EquipType.EQUIP_SHOES:
            panel.SandImage = image;
            panel.SandLabel = stringBuilder.ToString();
            break;
          case EquipType.EQUIP_RING:
            panel.GobletImage = image;
            panel.GobletLabel = stringBuilder.ToString();
            break;
          case EquipType.EQUIP_DRESS:
            panel.HeadImage = image;
            panel.HeadLabel = stringBuilder.ToString();
            break;
        }
      }
      panel.ConflicLabel = "";
      ProfileDetails profileDetails = panel;
      profileDetails.ConflicLabel = await this.CheckProfileConflict(profile);
      profileDetails = (ProfileDetails) null;
      ProfileDetails profileDetailPanel = panel;
      panel = (ProfileDetails) null;
      return profileDetailPanel;
    }

    private async Task<string> CheckProfileConflict(DbRelicProfile profile)
    {
      if (profile.WithScene.Count == 0 && profile.TeamContexts.Count == 0)
        return string.Empty;
      List<DbRelicProfile> otherProfiles1 = await Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) ((IIncludableQueryable<DbRelicProfile, IEnumerable<DbRelicItem>>) ((IQueryable<DbRelicProfile>) this._dbContext.RelicProfiles).Include<DbRelicProfile, ICollection<DbRelicItem>>((Expression<Func<DbRelicProfile, ICollection<DbRelicItem>>>) (p => p.RelicItems))).ThenInclude<DbRelicProfile, DbRelicItem, DbRelic>((Expression<Func<DbRelicItem, DbRelic>>) (r => r.Relic))).Include<DbRelicProfile, DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).ThenInclude<DbRelicProfile, DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar))).Include<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>((Expression<Func<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>>) (p => p.TeamContexts)), (Expression<Func<DbRelicProfile, bool>>) (p => p.Id != profile.Id && p.UserAvatar.UserUid == profile.UserAvatar.UserUid)).ToListAsync<DbRelicProfile>();
      ConcurrentBag<DbRelicProfile> teamConflicts = new ConcurrentBag<DbRelicProfile>();
      ConcurrentBag<DbRelicProfile> sceneConflicts = new ConcurrentBag<DbRelicProfile>();
      await Task.Run((Action) (() => Parallel.ForEach<DbRelicProfile>((IEnumerable<DbRelicProfile>) otherProfiles1, (Action<DbRelicProfile>) (otherProfiles =>
      {
        if (this.ProfileHasTeamConflict(profile, otherProfiles))
          teamConflicts.Add(otherProfiles);
        if (!this.ProfileHasSceneConflict(profile, otherProfiles))
          return;
        sceneConflicts.Add(otherProfiles);
      }))));
      if (teamConflicts.Count == 0 && sceneConflicts.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("冲突:");
      if (sceneConflicts.Count > 0)
      {
        stringBuilder.Append("场景(");
        stringBuilder.Append(Enumerable.Aggregate<string>(Enumerable.Select<DbRelicProfile, string>((IEnumerable<DbRelicProfile>) sceneConflicts, (Func<DbRelicProfile, string>) (p => p.ProfileName)), (Func<string, string, string>) ((a, b) => a + "," + b)));
        stringBuilder.Append(") ");
      }
      if (teamConflicts.Count > 0)
      {
        stringBuilder.Append("队伍(");
        stringBuilder.Append(Enumerable.Aggregate<string>(Enumerable.Select<DbRelicProfile, string>((IEnumerable<DbRelicProfile>) teamConflicts, (Func<DbRelicProfile, string>) (p => p.ProfileName)), (Func<string, string, string>) ((a, b) => a + "," + b)));
        stringBuilder.Append(") ");
      }
      return stringBuilder.ToString();
    }

    private bool ProfileHasTeamConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
    {
      if (Enumerable.Any<ulong>(Enumerable.Intersect<ulong>(Enumerable.Select<DbRelicItem, ulong>((IEnumerable<DbRelicItem>) profile.RelicItems, (Func<DbRelicItem, ulong>) (r => r.Guid)), Enumerable.Select<DbRelicItem, ulong>((IEnumerable<DbRelicItem>) otherProfile.RelicItems, (Func<DbRelicItem, ulong>) (r => r.Guid)))))
      {
        foreach (DbRelicProfileTeamContext teamContext1 in (IEnumerable<DbRelicProfileTeamContext>) profile.TeamContexts)
        {
          foreach (DbRelicProfileTeamContext teamContext2 in (IEnumerable<DbRelicProfileTeamContext>) otherProfile.TeamContexts)
          {
            if (Enumerable.Any<uint>(Enumerable.Intersect<uint>((IEnumerable<uint>) teamContext1.AvatarIds, (IEnumerable<uint>) teamContext2.AvatarIds)))
              return true;
          }
        }
      }
      return false;
    }

    private bool ProfileHasSceneConflict(DbRelicProfile profile, DbRelicProfile otherProfile)
    {
      return (long) profile.UserAvatar.Guid == (long) otherProfile.UserAvatar.Guid && Enumerable.Any<uint>(Enumerable.Intersect<uint>((IEnumerable<uint>) profile.WithScene, (IEnumerable<uint>) otherProfile.WithScene));
    }

    private void CheckProfileDetailSize()
    {
      List<ProfileDetails> list = Enumerable.ToList<ProfileDetails>(Enumerable.OfType<ProfileDetails>((IEnumerable) this.flpProfileDetails.Controls));
      Enumerable.FirstOrDefault<ScrollableControl>(Enumerable.OfType<ScrollableControl>((IEnumerable) this.flpProfileDetails.Controls));
      int width = this.flpProfileDetails.ClientSize.Width;
      Padding margin1 = this.flpProfileDetails.Margin;
      int horizontal1 = ((Padding) ref margin1).Horizontal;
      int minWidth = width - horizontal1;
      if (this.flpProfileDetails.VerticalScroll.Visible)
      {
        int num = this.flpProfileDetails.Width - SystemInformation.VerticalScrollBarWidth;
        Padding margin2 = this.flpProfileDetails.Margin;
        int horizontal2 = ((Padding) ref margin2).Horizontal;
        minWidth = num - horizontal2;
      }
      Action<ProfileDetails> action = (Action<ProfileDetails>) (x => x.MinimumSize = new Size(minWidth, (int) ((double) minWidth * (double) ProfileDetails.WidthToHeightScale)));
      list.ForEach(action);
    }

    private void OnServiceStatusChanged(object? sender, bool isOnline)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Action) (() => this.OnServiceStatusChanged(sender, isOnline)));
      }
      else
      {
        this._isOnline = isOnline;
        this._isOffline = !isOnline;
        this.btnAddPreset.Enabled = this._isOnline;
        this.serverStatus.Text = this._isOnline ? "服务在线" : "服务离线";
        this.serverStatus.Image = this._isOnline ? (Image) Resources.dot_green : (Image) Resources.dot_red;
        if (!this._isOffline)
          return;
        this._lastSelectedAvatarGuid = 0UL;
        this.ClearAvatarList();
        this.ClearProfileDetails();
        this.labelUid.Text = "UID: ";
        this.labelScene.Text = "场景: ";
      }
    }

    private void UpdateUidText(uint uid)
    {
      string str = uid.ToString();
      if (str.Length > 3)
      {
        StringBuilder stringBuilder = new StringBuilder(str);
        for (int index = 2; index < str.Length - 2; ++index)
          stringBuilder[index] = '*';
        str = stringBuilder.ToString();
      }
      this.labelUid.Text = "UID: " + str;
    }

    private async void OnUidChanged(object? sender, uint uid)
    {
      MainForm mainForm = this;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (mainForm.InvokeRequired))
      {
        mainForm.Invoke((Action) (() => this.OnUidChanged(sender, uid)));
      }
      else
      {
        mainForm.UpdateUidText(uid);
        if (uid == 0U)
        {
          mainForm._avatarProfileItemMap.Clear();
          mainForm.ClearProfileDetails();
        }
        else
        {
          mainForm._lastSelectedAvatarGuid = 0UL;
          await mainForm.PopulateAvatarList();
          mainForm.ClearProfileDetails();
        }
      }
    }

    private void OnSceneChanged(object? sender, uint sceneId)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Action) (() => this.OnSceneChanged(sender, sceneId)));
      }
      else
      {
        this.labelScene.Text = "场景: " + sceneId.ToString();
        if (sceneId == 0U)
        {
          this.ClearProfileDetails();
          this.ClearAvatarList();
        }
        this._lastSceneId = sceneId;
      }
    }

    private async void OnProfileRefresh(object? sender, EventArgs e)
    {
      MainForm mainForm = this;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (mainForm.InvokeRequired))
      {
        mainForm.Invoke((Action) (() => this.OnProfileRefresh(sender, e)));
      }
      else
      {
        if (mainForm._lastSelectedAvatarGuid == 0UL)
          return;
        await mainForm.PopulateAvatarList();
        await mainForm.ChangeRelicDetailDisplay();
      }
    }

    private void OnProfileConflict(object? sender, string message)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Action) (() => this.OnProfileConflict(sender, message)));
      }
      else
      {
        this.labelConflict.Text = message;
        this.btnConfirmConflict.Visible = true;
      }
    }

    private void ClearAvatarList()
    {
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) this.flpAvatars.Controls)).ForEach((Action<Control>) (c => c.Dispose()));
      this.flpAvatars.Controls.Clear();
    }

    private void ClearProfileDetails()
    {
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) this.flpProfileDetails.Controls)).ForEach((Action<Control>) (c => c.Dispose()));
      this.flpProfileDetails.Controls.Clear();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        ((IDisposable) this.components).Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.statusStrip = new StatusStrip();
      this.serverStatus = new ToolStripStatusLabel();
      this.menuStrip = new MenuStrip();
      this.menuAbout = new ToolStripMenuItem();
      this.groupBox = new GroupBox();
      this.splitContainer = new SplitContainer();
      this.tableLayoutLeft = new TableLayoutPanel();
      this.groupBoxAvatar = new GroupBox();
      this.flpAvatars = new FlowLayoutPanel();
      this.btnAddPreset = new Button();
      this.textBoxAvatarFilter = new TextBox();
      this.tableLayoutPanel = new TableLayoutPanel();
      this.groupBoxSelectedAvatar = new GroupBox();
      this.flpProfileDetails = new FlowLayoutPanel();
      this.labelUid = new Label();
      this.labelScene = new Label();
      this.cbAutoEquip = new CheckBox();
      this.labelConflict = new Label();
      this.btnConfirmConflict = new Button();
      this.cbEnableGameMessage = new CheckBox();
      this.btnRefreshMetadata = new Button();
      this.statusStrip.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.groupBox.SuspendLayout();
      ((ISupportInitialize) this.splitContainer).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tableLayoutLeft.SuspendLayout();
      this.groupBoxAvatar.SuspendLayout();
      this.tableLayoutPanel.SuspendLayout();
      this.groupBoxSelectedAvatar.SuspendLayout();
      this.SuspendLayout();
      this.statusStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.serverStatus
      });
      this.statusStrip.Location = new Point(0, 659);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new Size(1264, 22);
      this.statusStrip.SizingGrip = false;
      this.statusStrip.TabIndex = 0;
      this.statusStrip.Text = "statusStrip";
      this.serverStatus.Image = (Image) Resources.dot_red;
      this.serverStatus.Name = "serverStatus";
      this.serverStatus.Size = new Size(75, 17);
      this.serverStatus.Text = "服务离线";
      this.menuStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.menuAbout
      });
      this.menuStrip.Location = new Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new Size(1264, 24);
      this.menuStrip.TabIndex = 1;
      this.menuStrip.Text = "menuStrip";
      this.menuAbout.Name = "menuAbout";
      this.menuAbout.Size = new Size(45, 20);
      this.menuAbout.Text = "关于";
      this.menuAbout.Click += new EventHandler(this.menuAbout_Click);
      this.groupBox.Controls.Add((Control) this.splitContainer);
      this.groupBox.Dock = DockStyle.Fill;
      this.groupBox.Location = new Point(0, 24);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new Size(1264, 635);
      this.groupBox.TabIndex = 2;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "主页";
      this.splitContainer.Dock = DockStyle.Fill;
      this.splitContainer.Location = new Point(3, 19);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Panel1.Controls.Add((Control) this.tableLayoutLeft);
      this.splitContainer.Panel2.Controls.Add((Control) this.tableLayoutPanel);
      this.splitContainer.Size = new Size(1258, 613);
      this.splitContainer.SplitterDistance = 350;
      this.splitContainer.TabIndex = 0;
      this.tableLayoutLeft.ColumnCount = 1;
      this.tableLayoutLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutLeft.Controls.Add((Control) this.groupBoxAvatar, 0, 2);
      this.tableLayoutLeft.Controls.Add((Control) this.btnAddPreset, 0, 0);
      this.tableLayoutLeft.Controls.Add((Control) this.textBoxAvatarFilter, 0, 1);
      this.tableLayoutLeft.Dock = DockStyle.Fill;
      this.tableLayoutLeft.Location = new Point(0, 0);
      this.tableLayoutLeft.Name = "tableLayoutLeft";
      this.tableLayoutLeft.RowCount = 3;
      this.tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5f));
      this.tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 5f));
      this.tableLayoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 90f));
      this.tableLayoutLeft.Size = new Size(350, 613);
      this.tableLayoutLeft.TabIndex = 0;
      this.groupBoxAvatar.Controls.Add((Control) this.flpAvatars);
      this.groupBoxAvatar.Dock = DockStyle.Fill;
      this.groupBoxAvatar.Location = new Point(3, 63);
      this.groupBoxAvatar.Name = "groupBoxAvatar";
      this.groupBoxAvatar.Size = new Size(344, 547);
      this.groupBoxAvatar.TabIndex = 1;
      this.groupBoxAvatar.TabStop = false;
      this.groupBoxAvatar.Text = "预设角色列表";
      this.flpAvatars.AutoScroll = true;
      this.flpAvatars.BackColor = SystemColors.ControlLightLight;
      this.flpAvatars.BorderStyle = BorderStyle.FixedSingle;
      this.flpAvatars.Dock = DockStyle.Fill;
      this.flpAvatars.Location = new Point(3, 19);
      this.flpAvatars.Name = "flpAvatars";
      this.flpAvatars.Padding = new Padding(18, 10, 20, 20);
      this.flpAvatars.Size = new Size(338, 525);
      this.flpAvatars.TabIndex = 0;
      this.btnAddPreset.Dock = DockStyle.Fill;
      this.btnAddPreset.Location = new Point(5, 3);
      this.btnAddPreset.Margin = new Padding(5, 3, 5, 3);
      this.btnAddPreset.Name = "btnAddPreset";
      this.btnAddPreset.Size = new Size(340, 24);
      this.btnAddPreset.TabIndex = 0;
      this.btnAddPreset.Text = "添加预设";
      this.btnAddPreset.UseVisualStyleBackColor = true;
      this.btnAddPreset.Click += new EventHandler(this.btnAddPreset_Click);
      this.textBoxAvatarFilter.Dock = DockStyle.Fill;
      this.textBoxAvatarFilter.Location = new Point(6, 33);
      this.textBoxAvatarFilter.Margin = new Padding(6, 3, 6, 3);
      this.textBoxAvatarFilter.Name = "textBoxAvatarFilter";
      this.textBoxAvatarFilter.PlaceholderText = "列表过滤";
      this.textBoxAvatarFilter.Size = new Size(338, 23);
      this.textBoxAvatarFilter.TabIndex = 2;
      this.textBoxAvatarFilter.TextChanged += new EventHandler(this.textBoxAvatarFilter_TextChanged);
      this.tableLayoutPanel.ColumnCount = 5;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
      this.tableLayoutPanel.Controls.Add((Control) this.groupBoxSelectedAvatar, 0, 2);
      this.tableLayoutPanel.Controls.Add((Control) this.labelUid, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.labelScene, 0, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.cbAutoEquip, 1, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.labelConflict, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.btnConfirmConflict, 4, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.cbEnableGameMessage, 2, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.btnRefreshMetadata, 3, 1);
      this.tableLayoutPanel.Dock = DockStyle.Top;
      this.tableLayoutPanel.Location = new Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 3;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90f));
      this.tableLayoutPanel.Size = new Size(904, 613);
      this.tableLayoutPanel.TabIndex = 1;
      this.tableLayoutPanel.SetColumnSpan((Control) this.groupBoxSelectedAvatar, 5);
      this.groupBoxSelectedAvatar.Controls.Add((Control) this.flpProfileDetails);
      this.groupBoxSelectedAvatar.Dock = DockStyle.Fill;
      this.groupBoxSelectedAvatar.Location = new Point(3, 63);
      this.groupBoxSelectedAvatar.Name = "groupBoxSelectedAvatar";
      this.groupBoxSelectedAvatar.Size = new Size(898, 547);
      this.groupBoxSelectedAvatar.TabIndex = 0;
      this.groupBoxSelectedAvatar.TabStop = false;
      this.groupBoxSelectedAvatar.Text = "<角色名>";
      this.flpProfileDetails.AutoScroll = true;
      this.flpProfileDetails.BackColor = SystemColors.Window;
      this.flpProfileDetails.BorderStyle = BorderStyle.FixedSingle;
      this.flpProfileDetails.Dock = DockStyle.Fill;
      this.flpProfileDetails.FlowDirection = FlowDirection.TopDown;
      this.flpProfileDetails.Location = new Point(3, 19);
      this.flpProfileDetails.Name = "flpProfileDetails";
      this.flpProfileDetails.Size = new Size(892, 525);
      this.flpProfileDetails.TabIndex = 0;
      this.flpProfileDetails.WrapContents = false;
      this.labelUid.AutoSize = true;
      this.labelUid.Location = new Point(2, 7);
      this.labelUid.Margin = new Padding(2, 7, 7, 7);
      this.labelUid.Name = "labelUid";
      this.labelUid.Size = new Size(32, 15);
      this.labelUid.TabIndex = 1;
      this.labelUid.Text = "UID: ";
      this.labelScene.AutoSize = true;
      this.labelScene.Location = new Point(2, 37);
      this.labelScene.Margin = new Padding(2, 7, 7, 7);
      this.labelScene.Name = "labelScene";
      this.labelScene.Size = new Size(36, 15);
      this.labelScene.TabIndex = 3;
      this.labelScene.Text = "场景:";
      this.cbAutoEquip.AutoSize = true;
      this.cbAutoEquip.Location = new Point(48, 36);
      this.cbAutoEquip.Margin = new Padding(3, 6, 3, 3);
      this.cbAutoEquip.Name = "cbAutoEquip";
      this.cbAutoEquip.Size = new Size(78, 19);
      this.cbAutoEquip.TabIndex = 4;
      this.cbAutoEquip.Text = "自动装备";
      this.cbAutoEquip.UseVisualStyleBackColor = true;
      this.labelConflict.AutoSize = true;
      this.tableLayoutPanel.SetColumnSpan((Control) this.labelConflict, 3);
      this.labelConflict.Font = new Font("Segoe UI", 9f, (FontStyle) 1, (GraphicsUnit) 3, (byte) 0);
      this.labelConflict.ForeColor = Color.DarkRed;
      this.labelConflict.Location = new Point(47, 7);
      this.labelConflict.Margin = new Padding(2, 7, 7, 7);
      this.labelConflict.Name = "labelConflict";
      this.labelConflict.Size = new Size(40, 15);
      this.labelConflict.TabIndex = 5;
      this.labelConflict.Text = "label1";
      this.btnConfirmConflict.Location = new Point(827, 3);
      this.btnConfirmConflict.Name = "btnConfirmConflict";
      this.btnConfirmConflict.Size = new Size(74, 23);
      this.btnConfirmConflict.TabIndex = 6;
      this.btnConfirmConflict.Text = "确认";
      this.btnConfirmConflict.UseVisualStyleBackColor = true;
      this.btnConfirmConflict.Click += new EventHandler(this.btnConfirmConflict_Click);
      this.cbEnableGameMessage.AutoSize = true;
      this.cbEnableGameMessage.Location = new Point(132, 36);
      this.cbEnableGameMessage.Margin = new Padding(3, 6, 3, 3);
      this.cbEnableGameMessage.Name = "cbEnableGameMessage";
      this.cbEnableGameMessage.Size = new Size(117, 19);
      this.cbEnableGameMessage.TabIndex = 7;
      this.cbEnableGameMessage.Text = "启用游戏内通知";
      this.cbEnableGameMessage.UseVisualStyleBackColor = true;
      this.btnRefreshMetadata.AutoSize = true;
      this.btnRefreshMetadata.Location = new Point((int) byte.MaxValue, 33);
      this.btnRefreshMetadata.Name = "btnRefreshMetadata";
      this.btnRefreshMetadata.Size = new Size(82, 24);
      this.btnRefreshMetadata.TabIndex = 8;
      this.btnRefreshMetadata.Text = "刷新元数据";
      this.btnRefreshMetadata.UseVisualStyleBackColor = true;
      this.btnRefreshMetadata.Visible = false;
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(1264, 681);
      this.Controls.Add((Control) this.groupBox);
      this.Controls.Add((Control) this.statusStrip);
      this.Controls.Add((Control) this.menuStrip);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MainMenuStrip = this.menuStrip;
      this.MaximizeBox = false;
      this.Name = nameof (MainForm);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "圣遗物预设系统 by: Ex_M";
      this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new EventHandler(this.MainForm_Load);
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.groupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((ISupportInitialize) this.splitContainer).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.tableLayoutLeft.ResumeLayout(false);
      this.tableLayoutLeft.PerformLayout();
      this.groupBoxAvatar.ResumeLayout(false);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.groupBoxSelectedAvatar.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
