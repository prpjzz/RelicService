// Decompiled with JetBrains decompiler
// Type: RelicService.View.AddPresetForm
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Service;
using RelicService.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.View
{
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
    private 
    #nullable disable
    IContainer components;
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

    public AddPresetForm(
      #nullable enable
      AvatarService avatarService,
      EquipService equipService,
      StatusService statusService,
      EventManager eventManager,
      ResourceManager resourceManager,
      SqliteContext dbContext)
    {
      this.InitializeComponent();
      this._avatarService = avatarService;
      this._equipService = equipService;
      this._statusService = statusService;
      this._eventManager = eventManager;
      this._resourceManager = resourceManager;
      this._dbContext = dbContext;
      this._eventManager.OnFetchProgress += new EventHandler<FetchProgressEvent>(this.OnFetechProgressEvent);
      this._eventManager.OnUidChanged += new EventHandler<uint>(this.OnUidChanged);
    }

    private void AddPresetForm_Load(object sender, EventArgs e)
    {
      this.SetupBindings();
      this.tabControl.Enabled = false;
      this.statusLabel.Text = "启动中...";
    }

    private async void AddPresetForm_Shown(object sender, EventArgs e)
    {
      await this.OnSwitchToTeamTab();
      await this.PopulateAvatarCheckboxList();
      this.tabControl.Enabled = true;
    }

    private void AddPresetForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      this._eventManager.OnFetchProgress -= new EventHandler<FetchProgressEvent>(this.OnFetechProgressEvent);
      this._eventManager.OnUidChanged -= new EventHandler<uint>(this.OnUidChanged);
      Enumerable.ToList<uint>((IEnumerable<uint>) this._avatarImageRef).ForEach(new Action<uint>(this._resourceManager.FreeAvatarImage));
      Enumerable.ToList<uint>((IEnumerable<uint>) this._relicImageRef).ForEach(new Action<uint>(this._resourceManager.FreeRelicImage));
    }

    private async void tabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.tabControl.Enabled = false;
      switch (this.tabControl.SelectedIndex)
      {
        case 0:
          await this.OnSwitchToTeamTab();
          break;
        case 1:
          await this.OnSwitchToAllAvatarsTab();
          break;
      }
      this.tabControl.Enabled = true;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
      AddPresetForm addPresetForm = this;
      try
      {
        string text1 = addPresetForm.textBoxProfileName.Text;
        bool autoEquip = addPresetForm.cbEnableAutoEquip.Checked;
        bool activeWithScene = addPresetForm.cbActiveWithScene.Checked;
        string text2 = addPresetForm.tbSceneIds.Text;
        bool activeWithTeam = addPresetForm.cbActiveWithTeam.Checked;
        await addPresetForm.CreateAndSaveConfig(text1, autoEquip, activeWithScene, text2, activeWithTeam);
        int num1 = await addPresetForm._dbContext.SaveChangesAsync();
        int num2 = (int) MessageBox.Show("保存成功", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        // ISSUE: reference to a compiler-generated method
        Task.Run(new Func<Task>(addPresetForm.\u003CbtnSave_Click\u003Eb__25_0));
        addPresetForm.textBoxProfileName.Text = "";
        addPresetForm.cbEnableAutoEquip.Checked = false;
        addPresetForm.cbActiveWithScene.Checked = false;
        addPresetForm.tbSceneIds.Text = "";
        addPresetForm.cbActiveWithTeam.Checked = false;
        addPresetForm.btnClearSelectedTeam_Click(sender, e);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    private async void btnRefreshTeam_Click(object sender, EventArgs e)
    {
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) this.flpCurrentTeam.Controls)).ForEach((Action<Control>) (x => x.Dispose()));
      this.flpCurrentTeam.Controls.Clear();
      this._teamAvatarControlsMap.Clear();
      this.tabControl.Enabled = false;
      await this.OnSwitchToTeamTab();
      this.tabControl.Enabled = true;
    }

    private async void btnRefreshAllAvatar_Click(object sender, EventArgs e)
    {
      Enumerable.ToList<Control>(Enumerable.OfType<Control>((IEnumerable) this.flpAllAvatar.Controls)).ForEach((Action<Control>) (x => x.Dispose()));
      this.flpAllAvatar.Controls.Clear();
      this._allAvatarControlsMap.Clear();
      this.tabControl.Enabled = false;
      await this.OnSwitchToAllAvatarsTab();
      this.tabControl.Enabled = true;
    }

    private void btnUseCurrentTeam_Click(object sender, EventArgs e)
    {
      List<CheckBox> list1 = Enumerable.ToList<CheckBox>(Enumerable.OfType<CheckBox>((IEnumerable) this.flpAvatars.Controls));
      List<ulong> list2 = Enumerable.ToList<ulong>(Enumerable.Select<DbUserAvatar, ulong>(Enumerable.Where<DbUserAvatar>((IEnumerable<DbUserAvatar>) this._currentTeamCache, (Func<DbUserAvatar, bool>) (x => (long) x.Guid != (long) this._lastClickedTeamAvatar)), (Func<DbUserAvatar, ulong>) (x => x.Guid)));
      foreach (CheckBox checkBox in list1)
        checkBox.Checked = list2.Contains((ulong) checkBox.Tag);
    }

    private void btnClearSelectedTeam_Click(object sender, EventArgs e)
    {
      Enumerable.ToList<CheckBox>(Enumerable.OfType<CheckBox>((IEnumerable) this.flpAvatars.Controls)).ForEach((Action<CheckBox>) (x => x.Checked = false));
    }

    private void tbTeammateSearch_TextChanged(object sender, EventArgs e)
    {
      List<CheckBox> list = Enumerable.ToList<CheckBox>(Enumerable.OfType<CheckBox>((IEnumerable) this.flpAvatars.Controls));
      if (string.IsNullOrWhiteSpace(this.tbTeammateSearch.Text))
        list.ForEach((Action<CheckBox>) (x => x.Visible = true));
      else
        list.ForEach((Action<CheckBox>) (x => x.Visible = x.Text.Contains(this.tbTeammateSearch.Text)));
    }

    private void flpAvatars_OnCheckedChanged(object? sender, EventArgs e)
    {
      List<CheckBox> list = Enumerable.ToList<CheckBox>(Enumerable.OfType<CheckBox>((IEnumerable) this.flpAvatars.Controls));
      int checkedCount = Enumerable.Count<CheckBox>((IEnumerable<CheckBox>) list, (Func<CheckBox, bool>) (x => x.Checked));
      list.ForEach((Action<CheckBox>) (x => x.Enabled = checkedCount < 3 || x.Checked));
      if (checkedCount > 0)
        this.labelSelectedTeammates.Text = Enumerable.Aggregate<string>(Enumerable.Select<CheckBox, string>(Enumerable.Where<CheckBox>((IEnumerable<CheckBox>) list, (Func<CheckBox, bool>) (x => x.Checked)), (Func<CheckBox, string>) (x => x.Text)), (Func<string, string, string>) ((x, y) => x + "," + y));
      else
        this.labelSelectedTeammates.Text = "无";
    }

    private void SetupBindings()
    {
      this.groupBoxRight.Enabled = false;
      this.cbActiveWithScene.DataBindings.Add("Enabled", (object) this.cbEnableAutoEquip, "Checked");
      this.labelSceneId.DataBindings.Add("Enabled", (object) this.cbActiveWithScene, "Checked");
      this.tbSceneIds.DataBindings.Add("Enabled", (object) this.cbActiveWithScene, "Checked");
      this.cbActiveWithTeam.DataBindings.Add("Enabled", (object) this.cbEnableAutoEquip, "Checked");
      this.labelTeammates.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
      this.labelSelectedTeammates.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
      this.btnUseCurrentTeam.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
      this.btnClearSelectedTeam.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
      this.tbTeammateSearch.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
      this.flpAvatars.DataBindings.Add("Enabled", (object) this.cbActiveWithTeam, "Checked");
    }

    private void OnFetechProgressEvent(object? sender, FetchProgressEvent e)
    {
      if (this.InvokeRequired)
        this.Invoke((Action) (() => this.OnFetechProgressEvent(sender, e)));
      else if (e.Type == FetchType.None)
      {
        this.statusLabel.Text = "OK";
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("正在获取");
        stringBuilder.Append(e.Name);
        if (e.Total > 0U)
        {
          stringBuilder.Append(" (");
          stringBuilder.Append(e.Current);
          stringBuilder.Append("/");
          stringBuilder.Append(e.Total);
          stringBuilder.Append(")");
        }
        this.statusLabel.Text = stringBuilder.ToString();
      }
    }

    private void OnUidChanged(object? sender, uint uid)
    {
      if (this.InvokeRequired)
        this.Invoke((Action) (() => this.OnUidChanged(sender, uid)));
      else
        this.Close();
    }

    private async Task PopulateAvatarCheckboxList()
    {
      AddPresetForm addPresetForm = this;
      if (addPresetForm._statusService.CurrentUid == 0U)
        return;
      List<DbUserAvatar> userAvatars = await addPresetForm._avatarService.GetUserAvatars(addPresetForm._statusService.CurrentUid);
      if (userAvatars == null || userAvatars.Count <= 4)
      {
        await addPresetForm._avatarService.UpdateAllAvatarFromGame();
        userAvatars = await addPresetForm._avatarService.GetUserAvatars(addPresetForm._statusService.CurrentUid);
      }
      if (userAvatars == null)
        return;
      List<DbUserAvatar> list = Enumerable.ToList<DbUserAvatar>((IEnumerable<DbUserAvatar>) Enumerable.OrderBy<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) userAvatars, (Func<DbUserAvatar, uint>) (x => x.AvatarId)));
      addPresetForm._checkedListCache = list;
      addPresetForm.flpAvatars.Controls.Clear();
      foreach (DbUserAvatar dbUserAvatar in addPresetForm._checkedListCache)
      {
        CheckBox checkBox = new CheckBox();
        checkBox.Text = dbUserAvatar.Avatar.Name;
        checkBox.Tag = (object) dbUserAvatar.Guid;
        checkBox.Margin = new Padding(3, 3, 3, 3);
        checkBox.CheckedChanged += new EventHandler(addPresetForm.flpAvatars_OnCheckedChanged);
        addPresetForm.flpAvatars.Controls.Add((Control) checkBox);
        addPresetForm._avatarDataDict.Add(dbUserAvatar.Guid, dbUserAvatar);
      }
    }

    private async Task OnSwitchToTeamTab()
    {
      AddPresetForm addPresetForm = this;
      addPresetForm.btnUseCurrentTeam.Visible = true;
      addPresetForm.groupBoxRight.Enabled = addPresetForm._lastClickedTeamAvatar > 0UL;
      List<DbUserAvatar> currentTeam;
      Dictionary<ulong, List<RelicDataDto>> equipDict;
      if (addPresetForm.flpCurrentTeam.Controls.Count > 0)
      {
        currentTeam = (List<DbUserAvatar>) null;
        equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
      }
      else
      {
        addPresetForm._lastClickedTeamAvatar = 0UL;
        await addPresetForm._avatarService.UpdateTeamFromGame();
        currentTeam = await addPresetForm._avatarService.GetCurrentTeam();
        if (currentTeam == null)
        {
          addPresetForm.statusLabel.Text = "未获取到当前队伍信息";
          currentTeam = (List<DbUserAvatar>) null;
          equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
        }
        else
        {
          List<ulong> list = Enumerable.ToList<ulong>(Enumerable.Select<DbUserAvatar, ulong>((IEnumerable<DbUserAvatar>) currentTeam, (Func<DbUserAvatar, ulong>) (x => x.Guid)));
          equipDict = new Dictionary<ulong, List<RelicDataDto>>();
          foreach (ulong avatarGuid in list)
          {
            List<RelicDataDto> relicDataDtoList = await addPresetForm._equipService.UpdateAvatarEquipFromGame(avatarGuid);
            if (relicDataDtoList != null)
              equipDict.Add(avatarGuid, relicDataDtoList);
          }
          foreach (DbUserAvatar dbUserAvatar in currentTeam)
          {
            DbUserAvatar avatar = dbUserAvatar;
            AvatarRelicInfo infoPanel = await addPresetForm.CreateInfoPanel(avatar, equipDict);
            if (infoPanel != null)
            {
              infoPanel.Size = AddPresetForm.TeamInfoPanelSize;
              infoPanel.OnClickCallback = new Action<ulong>(addPresetForm.OnTeamAvatarEntryClick);
              int width = addPresetForm.flpCurrentTeam.ClientSize.Width;
              Padding margin = addPresetForm.flpCurrentTeam.Margin;
              int horizontal = ((Padding) ref margin).Horizontal;
              int num = width - horizontal;
              infoPanel.MinimumSize = new Size(num, (int) ((double) num * 0.16666));
              addPresetForm._teamAvatarControlsMap.Add(avatar.Guid, infoPanel);
              addPresetForm.flpCurrentTeam.Controls.Add((Control) infoPanel);
              avatar = (DbUserAvatar) null;
            }
          }
          addPresetForm._currentTeamCache = currentTeam;
          currentTeam = (List<DbUserAvatar>) null;
          equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
        }
      }
    }

    private async Task OnSwitchToAllAvatarsTab()
    {
      AddPresetForm addPresetForm = this;
      addPresetForm.btnUseCurrentTeam.Visible = false;
      addPresetForm.groupBoxRight.Enabled = addPresetForm._lastClickedAllAvatar > 0UL;
      List<DbUserAvatar> avatars;
      List<ulong> avatarGuidList;
      Dictionary<ulong, List<RelicDataDto>> equipDict;
      if (addPresetForm.flpAllAvatar.Controls.Count > 0)
      {
        avatars = (List<DbUserAvatar>) null;
        avatarGuidList = (List<ulong>) null;
        equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
      }
      else if (addPresetForm._statusService.CurrentUid == 0U)
      {
        avatars = (List<DbUserAvatar>) null;
        avatarGuidList = (List<ulong>) null;
        equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
      }
      else
      {
        addPresetForm._lastClickedAllAvatar = 0UL;
        await addPresetForm._avatarService.UpdateAllAvatarFromGame();
        avatars = await addPresetForm._avatarService.GetUserAvatars(addPresetForm._statusService.CurrentUid);
        if (avatars == null)
        {
          addPresetForm.statusLabel.Text = "未获取到当前用户人物信息";
          avatars = (List<DbUserAvatar>) null;
          avatarGuidList = (List<ulong>) null;
          equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
        }
        else
        {
          avatars = Enumerable.ToList<DbUserAvatar>((IEnumerable<DbUserAvatar>) Enumerable.OrderBy<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) avatars, (Func<DbUserAvatar, uint>) (x => x.AvatarId)));
          avatarGuidList = Enumerable.ToList<ulong>(Enumerable.Select<DbUserAvatar, ulong>((IEnumerable<DbUserAvatar>) avatars, (Func<DbUserAvatar, ulong>) (x => x.Guid)));
          addPresetForm._eventManager.OnFetchProgress -= new EventHandler<FetchProgressEvent>(addPresetForm.OnFetechProgressEvent);
          List<Task<(ulong, List<RelicDataDto>)>> taskList = new List<Task<(ulong, List<RelicDataDto>)>>();
          equipDict = new Dictionary<ulong, List<RelicDataDto>>();
          int processedCount = 1;
          foreach (ulong avatarGuid in avatarGuidList)
          {
            ToolStripStatusLabel statusLabel = addPresetForm.statusLabel;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
            interpolatedStringHandler.AppendLiteral("正在获取人物装备信息 (");
            interpolatedStringHandler.AppendFormatted<int>(processedCount);
            interpolatedStringHandler.AppendLiteral("/");
            interpolatedStringHandler.AppendFormatted<int>(avatarGuidList.Count);
            interpolatedStringHandler.AppendLiteral(")");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            statusLabel.Text = stringAndClear;
            List<RelicDataDto> relicDataDtoList = await addPresetForm._equipService.UpdateAvatarEquipFromGame(avatarGuid);
            if (relicDataDtoList != null)
              equipDict.Add(avatarGuid, relicDataDtoList);
            ++processedCount;
          }
          foreach (DbUserAvatar avatar in avatars)
          {
            AvatarRelicInfo infoPanel = await addPresetForm.CreateInfoPanel(avatar, equipDict);
            if (infoPanel != null)
            {
              infoPanel.Size = AddPresetForm.AllInfoPanelSize;
              infoPanel.OnClickCallback = new Action<ulong>(addPresetForm.OnAllAvatarEntryClick);
              int num1 = addPresetForm.flpAllAvatar.Width - SystemInformation.VerticalScrollBarWidth;
              Padding margin = addPresetForm.flpAllAvatar.Margin;
              int horizontal = ((Padding) ref margin).Horizontal;
              int num2 = num1 - horizontal;
              infoPanel.MinimumSize = new Size(num2, (int) ((double) num2 * 0.16666));
              addPresetForm._allAvatarControlsMap.Add(avatar.Guid, infoPanel);
              addPresetForm.flpAllAvatar.Controls.Add((Control) infoPanel);
            }
          }
          addPresetForm.statusLabel.Text = "OK";
          addPresetForm._eventManager.OnFetchProgress += new EventHandler<FetchProgressEvent>(addPresetForm.OnFetechProgressEvent);
          avatars = (List<DbUserAvatar>) null;
          avatarGuidList = (List<ulong>) null;
          equipDict = (Dictionary<ulong, List<RelicDataDto>>) null;
        }
      }
    }

    private async Task<AvatarRelicInfo?> CreateInfoPanel(
      DbUserAvatar avatar,
      Dictionary<ulong, List<RelicDataDto>> equipDict)
    {
      Image image1;
      if (!this._avatarImageCache.TryGetValue(avatar.AvatarId, ref image1))
      {
        image1 = await this._resourceManager.GetAvatarImage(avatar.AvatarId);
        if (image1 == null)
          return (AvatarRelicInfo) null;
        image1 = Utils.ResizeImage(image1, 70, 70);
        this._avatarImageRef.Add(avatar.AvatarId);
        this._avatarImageCache.Add(avatar.AvatarId, image1);
      }
      AvatarRelicInfo panel = new AvatarRelicInfo(avatar.Guid);
      panel.AvatarImage = image1;
      panel.Dock = DockStyle.Top;
      foreach (RelicDataDto relic in equipDict[avatar.Guid])
      {
        Image image2;
        if (!this._relicImageCache.TryGetValue(relic.ItemId, ref image2))
        {
          image2 = await this._resourceManager.GetRelicImage(relic.ItemId);
          if (image2 != null)
          {
            image2 = Utils.ResizeImage(image2, 70, 70);
            this._relicImageRef.Add(relic.ItemId);
            this._relicImageCache.Add(relic.ItemId, image2);
          }
          else
            continue;
        }
        string propStatTooltip = this.GetPropStatTooltip(relic);
        switch (relic.EquipType)
        {
          case EquipType.EQUIP_BRACER:
            panel.FlowerImage = image2;
            panel.FlowerTooltip = propStatTooltip;
            break;
          case EquipType.EQUIP_NECKLACE:
            panel.FeatherImage = image2;
            panel.FeatherTooltip = propStatTooltip;
            break;
          case EquipType.EQUIP_SHOES:
            panel.SandImage = image2;
            panel.SandTooltip = propStatTooltip;
            break;
          case EquipType.EQUIP_RING:
            panel.GobletImage = image2;
            panel.GobletTooltip = propStatTooltip;
            break;
          case EquipType.EQUIP_DRESS:
            panel.HeadImage = image2;
            panel.HeadTooltip = propStatTooltip;
            break;
        }
      }
      return panel;
    }

    private void OnTeamAvatarEntryClick(ulong avatarGuid)
    {
      if (this._lastClickedTeamAvatar != 0UL)
        this._teamAvatarControlsMap[this._lastClickedTeamAvatar].IsSelected = false;
      this._teamAvatarControlsMap[avatarGuid].IsSelected = true;
      this._lastClickedTeamAvatar = avatarGuid;
      this.groupBoxRight.Enabled = true;
    }

    private void OnAllAvatarEntryClick(ulong avatarGuid)
    {
      if (this._lastClickedAllAvatar != 0UL)
        this._allAvatarControlsMap[this._lastClickedAllAvatar].IsSelected = false;
      this._allAvatarControlsMap[avatarGuid].IsSelected = true;
      this._lastClickedAllAvatar = avatarGuid;
      this.groupBoxRight.Enabled = true;
    }

    private string GetPropStatTooltip(RelicDataDto relic)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      stringBuilder.Append(relic.Level - 1U);
      stringBuilder.Append("级] ");
      string str1 = Utils.FormatFightProp(relic.MainPropType, relic.MainPropValue);
      stringBuilder.Append(str1);
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      foreach (RelicAffixDto relicAffixDto in relic.AppendProp)
      {
        string str2 = Utils.FormatFightProp(relicAffixDto.PropType, relicAffixDto.PropValue);
        stringBuilder.Append(str2);
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    private async Task CreateAndSaveConfig(
      string profileName,
      bool autoEquip,
      bool activeWithScene,
      string sceneIds,
      bool activeWithTeam)
    {
      if (string.IsNullOrWhiteSpace(profileName))
        throw new Exception("预设名不能为空");
      ulong selectedAvatarGuid = this.tabControl.SelectedIndex == 0 ? this._lastClickedTeamAvatar : this._lastClickedAllAvatar;
      DbUserAvatar selectedAvatar = this._avatarDataDict[selectedAvatarGuid];
      if (await Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) this._dbContext.RelicProfiles, (Expression<Func<DbRelicProfile, bool>>) (x => x.ProfileName == profileName && x.AvatarGuid == selectedAvatarGuid)).FirstOrDefaultAsync<DbRelicProfile>() != null)
        throw new Exception("[" + selectedAvatar.Avatar.Name + "] 预设名已存在");
      List<DbRelicItem> dbRelicItemList = await this._equipService.UpdateAndSaveAvatarEquip(selectedAvatarGuid);
      if (dbRelicItemList.Count == 0)
        throw new Exception("不能创建无圣遗物的预设");
      DbRelicProfile entity = new DbRelicProfile();
      entity.ProfileName = profileName;
      entity.UserAvatar = selectedAvatar;
      entity.RelicItems = (ICollection<DbRelicItem>) dbRelicItemList;
      if (autoEquip)
      {
        if (activeWithScene)
        {
          string[] strArray = sceneIds.Replace(" ", "").Split(',', (StringSplitOptions) 0);
          if (strArray.Length == 0)
            throw new Exception("场景ID不能为空");
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            List<uint> list = Enumerable.ToList<uint>(Enumerable.Select<string, uint>((IEnumerable<string>) strArray, AddPresetForm.\u003C\u003EO.\u003C0\u003E__Parse ?? (AddPresetForm.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, uint>(uint.Parse))));
            entity.WithScene = list;
          }
          catch (Exception ex)
          {
            throw new Exception("场景ID必须为数字");
          }
        }
        if (activeWithTeam)
        {
          List<ulong> list1 = Enumerable.ToList<ulong>(Enumerable.Select<CheckBox, ulong>(Enumerable.Where<CheckBox>((IEnumerable<CheckBox>) Enumerable.ToList<CheckBox>(Enumerable.OfType<CheckBox>((IEnumerable) this.flpAvatars.Controls)), (Func<CheckBox, bool>) (x => x.Checked)), (Func<CheckBox, ulong>) (x => (ulong) x.Tag)));
          if (list1.Count > 3)
            throw new Exception("队伍人数不能超过3人");
          if (list1.Count == 0)
            throw new Exception("队伍人数不能为0");
          List<DbUserAvatar> list2 = Enumerable.ToList<DbUserAvatar>(Enumerable.Select<ulong, DbUserAvatar>(Enumerable.Where<ulong>((IEnumerable<ulong>) list1, new Func<ulong, bool>(this._avatarDataDict.ContainsKey)), (Func<ulong, DbUserAvatar>) (x => this._avatarDataDict[x])));
          this._dbContext.RelicProfileTeamContext.Add(new DbRelicProfileTeamContext()
          {
            AvatarIds = Enumerable.ToList<uint>(Enumerable.Select<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) list2, (Func<DbUserAvatar, uint>) (x => x.AvatarId))),
            Profile = entity
          });
        }
        if (activeWithScene | activeWithTeam)
          entity.AutoEquip = true;
      }
      this._dbContext.RelicProfiles.Add(entity);
      selectedAvatar = (DbUserAvatar) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        ((IDisposable) this.components).Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.splitContainer = new SplitContainer();
      this.tabControl = new TabControl();
      this.tabCurrentTeam = new TabPage();
      this.btnRefreshTeam = new Button();
      this.flpCurrentTeam = new FlowLayoutPanel();
      this.tabAllAvatar = new TabPage();
      this.btnRefreshAllAvatar = new Button();
      this.flpAllAvatar = new FlowLayoutPanel();
      this.groupBoxRight = new GroupBox();
      this.tableLayoutPanel = new TableLayoutPanel();
      this.textBoxProfileName = new TextBox();
      this.label1 = new Label();
      this.groupBoxAutoEquip = new GroupBox();
      this.tableLayoutAutoEquip = new TableLayoutPanel();
      this.cbEnableAutoEquip = new CheckBox();
      this.cbActiveWithScene = new CheckBox();
      this.labelSceneId = new Label();
      this.tbSceneIds = new TextBox();
      this.cbActiveWithTeam = new CheckBox();
      this.labelTeammates = new Label();
      this.labelSelectedTeammates = new Label();
      this.tbTeammateSearch = new TextBox();
      this.btnUseCurrentTeam = new Button();
      this.flpAvatars = new FlowLayoutPanel();
      this.btnClearSelectedTeam = new Button();
      this.btnSave = new Button();
      this.statusStrip = new StatusStrip();
      this.statusLabel = new ToolStripStatusLabel();
      ((ISupportInitialize) this.splitContainer).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tabCurrentTeam.SuspendLayout();
      this.tabAllAvatar.SuspendLayout();
      this.groupBoxRight.SuspendLayout();
      this.tableLayoutPanel.SuspendLayout();
      this.groupBoxAutoEquip.SuspendLayout();
      this.tableLayoutAutoEquip.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer.IsSplitterFixed = true;
      this.splitContainer.Location = new Point(0, 3);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Panel1.Controls.Add((Control) this.tabControl);
      this.splitContainer.Panel2.Controls.Add((Control) this.groupBoxRight);
      this.splitContainer.Size = new Size(800, 422);
      this.splitContainer.SplitterDistance = 500;
      this.splitContainer.TabIndex = 0;
      this.tabControl.Controls.Add((Control) this.tabCurrentTeam);
      this.tabControl.Controls.Add((Control) this.tabAllAvatar);
      this.tabControl.Location = new Point(5, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new Size(495, 419);
      this.tabControl.TabIndex = 0;
      this.tabControl.SelectedIndexChanged += new EventHandler(this.tabControl_SelectedIndexChanged);
      this.tabCurrentTeam.Controls.Add((Control) this.btnRefreshTeam);
      this.tabCurrentTeam.Controls.Add((Control) this.flpCurrentTeam);
      this.tabCurrentTeam.Location = new Point(4, 24);
      this.tabCurrentTeam.Name = "tabCurrentTeam";
      this.tabCurrentTeam.Padding = new Padding(3);
      this.tabCurrentTeam.Size = new Size(487, 391);
      this.tabCurrentTeam.TabIndex = 0;
      this.tabCurrentTeam.Text = "当前队伍";
      this.tabCurrentTeam.UseVisualStyleBackColor = true;
      this.btnRefreshTeam.Dock = DockStyle.Bottom;
      this.btnRefreshTeam.Location = new Point(3, 365);
      this.btnRefreshTeam.Name = "btnRefreshTeam";
      this.btnRefreshTeam.Size = new Size(481, 23);
      this.btnRefreshTeam.TabIndex = 1;
      this.btnRefreshTeam.Text = "刷新队伍";
      this.btnRefreshTeam.UseVisualStyleBackColor = true;
      this.btnRefreshTeam.Click += new EventHandler(this.btnRefreshTeam_Click);
      this.flpCurrentTeam.AutoScroll = true;
      this.flpCurrentTeam.Dock = DockStyle.Top;
      this.flpCurrentTeam.FlowDirection = FlowDirection.TopDown;
      this.flpCurrentTeam.Location = new Point(3, 3);
      this.flpCurrentTeam.Name = "flpCurrentTeam";
      this.flpCurrentTeam.Size = new Size(481, 360);
      this.flpCurrentTeam.TabIndex = 0;
      this.flpCurrentTeam.WrapContents = false;
      this.tabAllAvatar.Controls.Add((Control) this.btnRefreshAllAvatar);
      this.tabAllAvatar.Controls.Add((Control) this.flpAllAvatar);
      this.tabAllAvatar.Location = new Point(4, 24);
      this.tabAllAvatar.Name = "tabAllAvatar";
      this.tabAllAvatar.Padding = new Padding(3);
      this.tabAllAvatar.Size = new Size(487, 391);
      this.tabAllAvatar.TabIndex = 1;
      this.tabAllAvatar.Text = "所有人物";
      this.tabAllAvatar.UseVisualStyleBackColor = true;
      this.btnRefreshAllAvatar.Dock = DockStyle.Bottom;
      this.btnRefreshAllAvatar.Location = new Point(3, 365);
      this.btnRefreshAllAvatar.Name = "btnRefreshAllAvatar";
      this.btnRefreshAllAvatar.Size = new Size(481, 23);
      this.btnRefreshAllAvatar.TabIndex = 1;
      this.btnRefreshAllAvatar.Text = "刷新列表";
      this.btnRefreshAllAvatar.UseVisualStyleBackColor = true;
      this.btnRefreshAllAvatar.Click += new EventHandler(this.btnRefreshAllAvatar_Click);
      this.flpAllAvatar.AutoScroll = true;
      this.flpAllAvatar.Dock = DockStyle.Top;
      this.flpAllAvatar.FlowDirection = FlowDirection.TopDown;
      this.flpAllAvatar.Location = new Point(3, 3);
      this.flpAllAvatar.Name = "flpAllAvatar";
      this.flpAllAvatar.Size = new Size(481, 360);
      this.flpAllAvatar.TabIndex = 0;
      this.flpAllAvatar.WrapContents = false;
      this.groupBoxRight.Controls.Add((Control) this.tableLayoutPanel);
      this.groupBoxRight.Location = new Point(0, 0);
      this.groupBoxRight.Name = "groupBoxRight";
      this.groupBoxRight.Size = new Size(290, 419);
      this.groupBoxRight.TabIndex = 0;
      this.groupBoxRight.TabStop = false;
      this.groupBoxRight.Text = "预设设置";
      this.tableLayoutPanel.ColumnCount = 2;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.Controls.Add((Control) this.textBoxProfileName, 1, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.label1, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.groupBoxAutoEquip, 0, 1);
      this.tableLayoutPanel.Controls.Add((Control) this.btnSave, 0, 2);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(3, 19);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 3;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel.Size = new Size(284, 397);
      this.tableLayoutPanel.TabIndex = 0;
      this.textBoxProfileName.Dock = DockStyle.Fill;
      this.textBoxProfileName.Location = new Point(63, 3);
      this.textBoxProfileName.Name = "textBoxProfileName";
      this.textBoxProfileName.Size = new Size(218, 23);
      this.textBoxProfileName.TabIndex = 0;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(7, 7);
      this.label1.Margin = new Padding(7);
      this.label1.Name = "label1";
      this.label1.Size = new Size(46, 15);
      this.label1.TabIndex = 1;
      this.label1.Text = "预设名";
      this.tableLayoutPanel.SetColumnSpan((Control) this.groupBoxAutoEquip, 2);
      this.groupBoxAutoEquip.Controls.Add((Control) this.tableLayoutAutoEquip);
      this.groupBoxAutoEquip.Dock = DockStyle.Fill;
      this.groupBoxAutoEquip.Location = new Point(10, 39);
      this.groupBoxAutoEquip.Margin = new Padding(10);
      this.groupBoxAutoEquip.Name = "groupBoxAutoEquip";
      this.groupBoxAutoEquip.Size = new Size(264, 319);
      this.groupBoxAutoEquip.TabIndex = 2;
      this.groupBoxAutoEquip.TabStop = false;
      this.groupBoxAutoEquip.Text = "自动装备 [ 可选 ]";
      this.tableLayoutAutoEquip.ColumnCount = 2;
      this.tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutAutoEquip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutAutoEquip.Controls.Add((Control) this.cbEnableAutoEquip, 0, 0);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.cbActiveWithScene, 0, 1);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.labelSceneId, 0, 2);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.tbSceneIds, 1, 2);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.cbActiveWithTeam, 0, 3);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.labelTeammates, 0, 4);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.labelSelectedTeammates, 1, 4);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.tbTeammateSearch, 1, 5);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.btnUseCurrentTeam, 0, 5);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.flpAvatars, 1, 6);
      this.tableLayoutAutoEquip.Controls.Add((Control) this.btnClearSelectedTeam, 0, 6);
      this.tableLayoutAutoEquip.Dock = DockStyle.Fill;
      this.tableLayoutAutoEquip.Location = new Point(3, 19);
      this.tableLayoutAutoEquip.Name = "tableLayoutAutoEquip";
      this.tableLayoutAutoEquip.RowCount = 7;
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle());
      this.tableLayoutAutoEquip.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutAutoEquip.Size = new Size(258, 297);
      this.tableLayoutAutoEquip.TabIndex = 0;
      this.cbEnableAutoEquip.AutoSize = true;
      this.cbEnableAutoEquip.Location = new Point(7, 4);
      this.cbEnableAutoEquip.Margin = new Padding(7, 4, 7, 4);
      this.cbEnableAutoEquip.Name = "cbEnableAutoEquip";
      this.cbEnableAutoEquip.Size = new Size(52, 19);
      this.cbEnableAutoEquip.TabIndex = 0;
      this.cbEnableAutoEquip.Text = "启用";
      this.cbEnableAutoEquip.UseVisualStyleBackColor = true;
      this.cbActiveWithScene.AutoSize = true;
      this.cbActiveWithScene.Location = new Point(7, 31);
      this.cbActiveWithScene.Margin = new Padding(7, 4, 5, 4);
      this.cbActiveWithScene.Name = "cbActiveWithScene";
      this.cbActiveWithScene.Size = new Size(91, 19);
      this.cbActiveWithScene.TabIndex = 1;
      this.cbActiveWithScene.Text = "按场景激活";
      this.cbActiveWithScene.UseVisualStyleBackColor = true;
      this.labelSceneId.AutoSize = true;
      this.labelSceneId.Location = new Point(7, 61);
      this.labelSceneId.Margin = new Padding(7);
      this.labelSceneId.Name = "labelSceneId";
      this.labelSceneId.Size = new Size(44, 15);
      this.labelSceneId.TabIndex = 2;
      this.labelSceneId.Text = "场景ID";
      this.tbSceneIds.Dock = DockStyle.Fill;
      this.tbSceneIds.Location = new Point(106, 57);
      this.tbSceneIds.Name = "tbSceneIds";
      this.tbSceneIds.PlaceholderText = "例: 3,1016,1056";
      this.tbSceneIds.Size = new Size(149, 23);
      this.tbSceneIds.TabIndex = 3;
      this.cbActiveWithTeam.AutoSize = true;
      this.cbActiveWithTeam.Location = new Point(7, 87);
      this.cbActiveWithTeam.Margin = new Padding(7, 4, 5, 4);
      this.cbActiveWithTeam.Name = "cbActiveWithTeam";
      this.cbActiveWithTeam.Size = new Size(91, 19);
      this.cbActiveWithTeam.TabIndex = 4;
      this.cbActiveWithTeam.Text = "按队伍激活";
      this.cbActiveWithTeam.UseVisualStyleBackColor = true;
      this.labelTeammates.AutoSize = true;
      this.labelTeammates.Location = new Point(7, 117);
      this.labelTeammates.Margin = new Padding(7);
      this.labelTeammates.Name = "labelTeammates";
      this.labelTeammates.Size = new Size(33, 15);
      this.labelTeammates.TabIndex = 5;
      this.labelTeammates.Text = "队友";
      this.labelSelectedTeammates.AutoSize = true;
      this.labelSelectedTeammates.Location = new Point(103, 117);
      this.labelSelectedTeammates.Margin = new Padding(0, 7, 7, 7);
      this.labelSelectedTeammates.Name = "labelSelectedTeammates";
      this.labelSelectedTeammates.Size = new Size(20, 15);
      this.labelSelectedTeammates.TabIndex = 6;
      this.labelSelectedTeammates.Text = "无";
      this.tbTeammateSearch.Dock = DockStyle.Fill;
      this.tbTeammateSearch.Location = new Point(106, 142);
      this.tbTeammateSearch.Name = "tbTeammateSearch";
      this.tbTeammateSearch.PlaceholderText = "人物搜索";
      this.tbTeammateSearch.Size = new Size(149, 23);
      this.tbTeammateSearch.TabIndex = 9;
      this.tbTeammateSearch.TextChanged += new EventHandler(this.tbTeammateSearch_TextChanged);
      this.btnUseCurrentTeam.Dock = DockStyle.Fill;
      this.btnUseCurrentTeam.Location = new Point(3, 142);
      this.btnUseCurrentTeam.Name = "btnUseCurrentTeam";
      this.btnUseCurrentTeam.Size = new Size(97, 23);
      this.btnUseCurrentTeam.TabIndex = 10;
      this.btnUseCurrentTeam.Text = "使用当前队伍";
      this.btnUseCurrentTeam.UseVisualStyleBackColor = true;
      this.btnUseCurrentTeam.Click += new EventHandler(this.btnUseCurrentTeam_Click);
      this.flpAvatars.AutoScroll = true;
      this.flpAvatars.BackColor = SystemColors.Window;
      this.flpAvatars.BorderStyle = BorderStyle.FixedSingle;
      this.flpAvatars.Dock = DockStyle.Fill;
      this.flpAvatars.FlowDirection = FlowDirection.TopDown;
      this.flpAvatars.Location = new Point(106, 171);
      this.flpAvatars.Name = "flpAvatars";
      this.flpAvatars.Size = new Size(149, 123);
      this.flpAvatars.TabIndex = 11;
      this.flpAvatars.WrapContents = false;
      this.btnClearSelectedTeam.Dock = DockStyle.Top;
      this.btnClearSelectedTeam.Location = new Point(3, 171);
      this.btnClearSelectedTeam.Name = "btnClearSelectedTeam";
      this.btnClearSelectedTeam.Size = new Size(97, 23);
      this.btnClearSelectedTeam.TabIndex = 12;
      this.btnClearSelectedTeam.Text = "清空选择";
      this.btnClearSelectedTeam.UseVisualStyleBackColor = true;
      this.btnClearSelectedTeam.Click += new EventHandler(this.btnClearSelectedTeam_Click);
      this.tableLayoutPanel.SetColumnSpan((Control) this.btnSave, 2);
      this.btnSave.Dock = DockStyle.Fill;
      this.btnSave.Location = new Point(3, 371);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(278, 23);
      this.btnSave.TabIndex = 3;
      this.btnSave.Text = "保存";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.statusStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.statusLabel
      });
      this.statusStrip.Location = new Point(0, 428);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new Size(800, 22);
      this.statusStrip.SizingGrip = false;
      this.statusStrip.TabIndex = 1;
      this.statusStrip.Text = "statusStrip1";
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new Size(49, 17);
      this.statusLabel.Text = "<状态>";
      this.AutoScaleDimensions = new SizeF(7f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(800, 450);
      this.Controls.Add((Control) this.statusStrip);
      this.Controls.Add((Control) this.splitContainer);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AddPresetForm);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "添加预设";
      this.FormClosed += new FormClosedEventHandler(this.AddPresetForm_FormClosed);
      this.Load += new EventHandler(this.AddPresetForm_Load);
      this.Shown += new EventHandler(this.AddPresetForm_Shown);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((ISupportInitialize) this.splitContainer).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.tabCurrentTeam.ResumeLayout(false);
      this.tabAllAvatar.ResumeLayout(false);
      this.groupBoxRight.ResumeLayout(false);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.groupBoxAutoEquip.ResumeLayout(false);
      this.tableLayoutAutoEquip.ResumeLayout(false);
      this.tableLayoutAutoEquip.PerformLayout();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
