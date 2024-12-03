// Decompiled with JetBrains decompiler
// Type: RelicService.Service.AvatarService
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace RelicService.Service
{
  internal class AvatarService
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly EventManager _eventManager;
    private readonly Network _network;
    private readonly StatusService _statusService;
    private readonly SqliteContext _dbContext;
    private uint _fetchCurrent;
    private uint _fetchTotal;
    private FetchType _fetchType;
    private readonly object _lock = new object();

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

    public AvatarService(
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

    public async Task<DbAvatar?> GetAvatarMetadata(uint avatarId)
    {
      return await this._dbContext.Avatars.FindAsync((object) avatarId);
    }

    public async Task<DbUserAvatar?> GetUserAvatarByGuid(ulong avatarGuid)
    {
      return await Queryable.Where<DbUserAvatar>((IQueryable<DbUserAvatar>) ((IQueryable<DbUserAvatar>) this._dbContext.UserAvatars).Include<DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar)), (Expression<Func<DbUserAvatar, bool>>) (ua => ua.Guid == avatarGuid)).FirstOrDefaultAsync<DbUserAvatar>(this._cts.Token);
    }

    public async Task<List<DbUserAvatar>?> GetUserAvatars(uint uid)
    {
      return await Queryable.Where<DbUserAvatar>((IQueryable<DbUserAvatar>) ((IQueryable<DbUserAvatar>) this._dbContext.UserAvatars).Include<DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (x => x.Avatar)), (Expression<Func<DbUserAvatar, bool>>) (x => x.UserUid == uid)).ToListAsync<DbUserAvatar>(this._cts.Token);
    }

    public async Task<List<DbUserAvatar>?> GetCurrentTeam()
    {
      try
      {
        AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(await this._network.GetCurrentTeamAsync());
        if (avatarListDto == null || avatarListDto.AvatarGuids == null)
          return (List<DbUserAvatar>) null;
        List<DbUserAvatar> dbAvatars = new List<DbUserAvatar>();
        foreach (ulong avatarGuid in avatarListDto.AvatarGuids)
        {
          DbUserAvatar async = await this._dbContext.UserAvatars.FindAsync((object) avatarGuid);
          if (async != null)
            dbAvatars.Add(async);
        }
        return dbAvatars;
      }
      catch (Exception ex)
      {
        return (List<DbUserAvatar>) null;
      }
    }

    public async Task UpdateTeamFromGame()
    {
      try
      {
        Monitor.Enter(this._lock);
        string currentTeamAsync = await this._network.GetCurrentTeamAsync();
        AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(currentTeamAsync);
        if (avatarListDto == null || avatarListDto.AvatarGuids == null)
          throw new Exception("failed to deserialize: " + currentTeamAsync);
        await this.UpdateDataAndResources(avatarListDto.AvatarGuids);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Error updating team data: " + ex.Message, "Error");
      }
      finally
      {
        this.FinishFetch();
        Monitor.Exit(this._lock);
      }
    }

    public async Task UpdateAllAvatarFromGame()
    {
      try
      {
        Monitor.Enter(this._lock);
        string allAvatarsAsync = await this._network.GetAllAvatarsAsync();
        AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(allAvatarsAsync);
        if (avatarListDto == null || avatarListDto.AvatarGuids == null)
          throw new Exception("failed to deserialize: " + allAvatarsAsync);
        await this.UpdateDataAndResources(avatarListDto.AvatarGuids);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Error updating avatar data: " + ex.Message, "Error");
      }
      finally
      {
        this.FinishFetch();
        Monitor.Exit(this._lock);
      }
    }

    private async Task UpdateDataAndResources(List<ulong> avatarGuidList)
    {
      await this.UpdateAllAvatarData(await this.FetchAvatarDataFromGame(avatarGuidList));
      int num = await this._dbContext.SaveChangesAsync(this._cts.Token);
    }

    private async Task<List<AvatarDataDto>> FetchAvatarDataFromGame(List<ulong> avatarGuidList)
    {
      this.ResetFetchState();
      this._fetchType = FetchType.AvatarMetadata;
      this.FetchTotal = (uint) avatarGuidList.Count;
      List<Exception> exceptions = new List<Exception>();
      List<Task<AvatarDataDto>> tasks = new List<Task<AvatarDataDto>>();
      avatarGuidList.ForEach((Action<ulong>) (avatarGuid => tasks.Add(this.FetchAvatarDataFromGame(avatarGuid).ContinueWith<AvatarDataDto>((Func<Task<AvatarDataDto>, AvatarDataDto>) (t =>
      {
        if (((Task) t).IsFaulted)
          exceptions.Add((Exception) ((Task) t).Exception);
        return t.Result;
      })))));
      if (exceptions.Count > 0)
        throw exceptions[0];
      return Enumerable.ToList<AvatarDataDto>(Enumerable.Select<AvatarDataDto, AvatarDataDto>(Enumerable.Where<AvatarDataDto>((IEnumerable<AvatarDataDto>) await Task.WhenAll<AvatarDataDto>((IEnumerable<Task<AvatarDataDto>>) tasks), (Func<AvatarDataDto, bool>) (x => x != null)), (Func<AvatarDataDto, AvatarDataDto>) (x => x)));
    }

    private async Task<AvatarDataDto?> FetchAvatarDataFromGame(ulong avatarGuid)
    {
      string avatarInfoAsync = await this._network.GetAvatarInfoAsync(avatarGuid);
      AvatarDataDto avatarDataDto = JsonConvert.DeserializeObject<AvatarDataDto>(avatarInfoAsync);
      if (avatarDataDto == null)
        throw new Exception("failed to deserialize: " + avatarInfoAsync);
      this.FetchCurrent++;
      return avatarDataDto;
    }

    private async Task UpdateAllAvatarData(List<AvatarDataDto> avatarDataList)
    {
      this.ResetFetchState();
      this._fetchType = FetchType.AvatarResource;
      this.FetchTotal = (uint) avatarDataList.Count;
      foreach (AvatarDataDto avatarData in avatarDataList)
      {
        await this.UpdateAvatarData(avatarData);
        this.FetchCurrent++;
      }
    }

    private async Task UpdateAvatarData(AvatarDataDto avatarData)
    {
      DbAvatar dbAvatar = await this._dbContext.Avatars.FindAsync((object) avatarData.AvatarId);
      if (dbAvatar == null)
      {
        DbAvatar dbAvatar1 = new DbAvatar();
        dbAvatar1.AvatarId = avatarData.AvatarId;
        DbAvatar dbAvatar2 = dbAvatar1;
        dbAvatar2.Name = await this.GetAvatarName(avatarData.NameTextId);
        dbAvatar1.TextId = avatarData.NameTextId;
        dbAvatar1.IconName = avatarData.IconName;
        DbAvatar dbAvatar3 = dbAvatar1;
        dbAvatar3.IconBase64 = await this.GetAvatarImage(avatarData.IconName);
        dbAvatar = dbAvatar1;
        dbAvatar2 = (DbAvatar) null;
        dbAvatar3 = (DbAvatar) null;
        dbAvatar1 = (DbAvatar) null;
        this._dbContext.Avatars.Add(dbAvatar);
      }
      else
        this._dbContext.Avatars.Attach(dbAvatar);
      await this.UpdateUserAvatarData(dbAvatar, avatarData);
    }

    private async Task UpdateUserAvatarData(DbAvatar dbAvatar, AvatarDataDto avatarData)
    {
      uint uid = this._statusService.CurrentUid;
      if (await this._dbContext.UserAvatars.FindAsync((object) avatarData.Guid) != null)
        return;
      this._dbContext.UserAvatars.Add(new DbUserAvatar()
      {
        Guid = avatarData.Guid,
        AvatarId = dbAvatar.AvatarId,
        UserUid = uid
      });
    }

    private async Task<string> GetAvatarName(uint nameTextId)
    {
      return await this._network.GetTextAsync(nameTextId);
    }

    private async Task<string> GetAvatarImage(string imageName)
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
