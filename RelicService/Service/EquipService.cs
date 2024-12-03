// Decompiled with JetBrains decompiler
// Type: RelicService.Service.EquipService
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.Service
{
  internal class EquipService
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly EventManager _eventManager;
    private readonly Network _network;
    private StatusService _statusService;
    private readonly SqliteContext _dbContext;
    private uint _fetchCurrent;
    private uint _fetchTotal;
    private FetchType _fetchType;
    private readonly object _lock = new object();
    private readonly HashSet<uint> _pendingRelics = new HashSet<uint>();

    public bool IsBusy
    {
      get
      {
        int num = Monitor.TryEnter(this._lock) ? 1 : 0;
        if (num != 0)
          Monitor.Exit(this._lock);
        return num == 0;
      }
      private set
      {
      }
    }

    public uint FetchCurrent
    {
      get => this._fetchCurrent;
      private set
      {
        int num = (int) Interlocked.Exchange(ref this._fetchCurrent, value);
        this._eventManager.FireEventAsync(EventId.EvtFetchProgress, (object) new FetchProgressEvent(this._fetchType, value, this._fetchTotal));
      }
    }

    public uint FetchTotal
    {
      get => this._fetchTotal;
      private set
      {
        int num = (int) Interlocked.Exchange(ref this._fetchTotal, value);
        this._eventManager.FireEventAsync(EventId.EvtFetchProgress, (object) new FetchProgressEvent(this._fetchType, this._fetchCurrent, value));
      }
    }

    public EquipService(
      EventManager eventManager,
      Network network,
      StatusService statusService,
      SqliteContext dbContext)
    {
      this._eventManager = eventManager;
      this._network = network;
      this._statusService = statusService;
      this._dbContext = dbContext;
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
    }

    public async Task<bool> WearEquip(ulong avatarGuid, ulong relicGuid)
    {
      try
      {
        await this._network.UpdateAvatarEquipAsync(avatarGuid, relicGuid);
        return true;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return false;
      }
    }

    public async Task<List<RelicDataDto>> UpdateAvatarEquipFromGame(ulong avatarGuid)
    {
      try
      {
        this._fetchType = FetchType.RelicMetadata;
        this.FetchTotal = 0U;
        List<RelicDataDto> relicList = await this.FetchAvatarEquipFromGame(avatarGuid);
        await this.UpdateRelicData(relicList);
        int num = await this._dbContext.SaveChangesAsync();
        return relicList;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return new List<RelicDataDto>();
      }
      finally
      {
        this.FinishFetch();
      }
    }

    public async Task<List<DbRelicItem>> UpdateAndSaveAvatarEquip(ulong avatarGuid)
    {
      try
      {
        this._fetchType = FetchType.RelicMetadata;
        this.FetchTotal = 0U;
        List<RelicDataDto> relicList = await this.FetchAvatarEquipFromGame(avatarGuid);
        await this.UpdateRelicData(relicList);
        List<DbRelicItem> result = await this.UpdateRelicDetails(relicList);
        int num = await this._dbContext.SaveChangesAsync();
        return result;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return new List<DbRelicItem>();
      }
      finally
      {
        this.FinishFetch();
      }
    }

    private async Task<List<RelicDataDto>> FetchAvatarEquipFromGame(ulong guid)
    {
      try
      {
        string avatarRelicsAsync = await this._network.GetAvatarRelicsAsync(guid);
        return JsonConvert.DeserializeObject<List<RelicDataDto>>(avatarRelicsAsync) ?? throw new Exception("failed to deserialize: " + avatarRelicsAsync);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return new List<RelicDataDto>();
      }
    }

    private async Task UpdateRelicData(List<RelicDataDto> relicList)
    {
      try
      {
        this._pendingRelics.Clear();
        this._fetchType = FetchType.RelicMetadata;
        this.FetchTotal = (uint) relicList.Count;
        List<Task> tasks = new List<Task>();
        relicList.ForEach((Action<RelicDataDto>) (relic => tasks.Add(this.UpdateRelicMetadata(relic))));
        await Task.WhenAll((IEnumerable<Task>) tasks);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      finally
      {
        this.FinishFetch();
      }
    }

    private async Task UpdateRelicMetadata(RelicDataDto relic)
    {
      if (this._pendingRelics.Contains(relic.ItemId))
        return;
      if (await this._dbContext.Relics.FindAsync((object) relic.ItemId) != null)
        return;
      DbRelic dbRelic1 = new DbRelic();
      dbRelic1.ItemId = relic.ItemId;
      dbRelic1.TextId = relic.NameTextId;
      DbRelic dbRelic2 = dbRelic1;
      dbRelic2.Name = await this.GetEquipName(relic.NameTextId);
      dbRelic1.IconName = relic.IconName;
      DbRelic dbRelic3 = dbRelic1;
      dbRelic3.IconBase64 = await this.GetEquipImage(relic.IconName);
      DbRelic entity = dbRelic1;
      dbRelic2 = (DbRelic) null;
      dbRelic3 = (DbRelic) null;
      dbRelic1 = (DbRelic) null;
      this._dbContext.Relics.Add(entity);
      this._pendingRelics.Add(relic.ItemId);
      this.FetchCurrent++;
    }

    private async Task<List<DbRelicItem>> UpdateRelicDetails(List<RelicDataDto> relicDatas)
    {
      List<DbRelicItem> dbRelicItems = new List<DbRelicItem>();
      foreach (RelicDataDto relicData in relicDatas)
        dbRelicItems.Add(await this.UpdateRelicDetail(relicData));
      List<DbRelicItem> dbRelicItemList = dbRelicItems;
      dbRelicItems = (List<DbRelicItem>) null;
      return dbRelicItemList;
    }

    private async Task<DbRelicItem> UpdateRelicDetail(RelicDataDto relic)
    {
      DbRelicItem dbRelicItem = await Queryable.Where<DbRelicItem>((IQueryable<DbRelicItem>) ((IQueryable<DbRelicItem>) this._dbContext.RelicItems).Include<DbRelicItem, ICollection<DbRelicAffix>>((Expression<Func<DbRelicItem, ICollection<DbRelicAffix>>>) (r => r.Affixes)), (Expression<Func<DbRelicItem, bool>>) (r => r.Guid == relic.Guid)).FirstOrDefaultAsync<DbRelicItem>();
      if (dbRelicItem != null)
        return dbRelicItem;
      DbRelic async = await this._dbContext.Relics.FindAsync((object) relic.ItemId);
      if (async == null)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
        interpolatedStringHandler.AppendLiteral("relic metadata not found: ");
        interpolatedStringHandler.AppendFormatted<uint>(relic.ItemId);
        throw new Exception(interpolatedStringHandler.ToStringAndClear());
      }
      DbRelicItem entity = new DbRelicItem()
      {
        Guid = relic.Guid,
        Level = relic.Level,
        MainPropId = relic.MainPropId,
        RankLevel = relic.RankLevel,
        EquipType = relic.EquipType,
        MainPropType = relic.MainPropType,
        MainPropValue = relic.MainPropValue,
        Relic = async
      };
      foreach (RelicAffixDto relicAffixDto in relic.AppendProp)
      {
        DbRelicAffix dbRelicAffix = new DbRelicAffix()
        {
          PropType = relicAffixDto.PropType,
          PropValue = relicAffixDto.PropValue,
          Relic = entity
        };
        entity.Affixes.Add(dbRelicAffix);
      }
      this._dbContext.RelicItems.Add(entity);
      return entity;
    }

    private async Task<string> GetEquipName(uint nameTextId)
    {
      return await this._network.GetTextAsync(nameTextId);
    }

    private async Task<string> GetEquipImage(string imageName)
    {
      return await this._network.GetItemImageAsync(imageName);
    }

    private void FinishFetch()
    {
      this.ResetFetchState();
      this._fetchType = FetchType.None;
      this._eventManager.FireEventAsync(EventId.EvtFetchProgress, (object) new FetchProgressEvent(FetchType.None, 0U, 0U));
    }

    private void ResetFetchState()
    {
      this._fetchType = FetchType.None;
      this.FetchCurrent = 0U;
      this.FetchTotal = 0U;
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
