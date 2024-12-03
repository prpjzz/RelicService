// Decompiled with JetBrains decompiler
// Type: RelicService.Service.AutoEquipService
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Service
{
  internal class AutoEquipService
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private EventManager _eventManager;
    private Network _network;
    private AvatarService _avatarService;
    private StatusService _statusService;
    private EquipService _equipService;
    private GameMessageService _gameMessageService;
    private SqliteContext _dbContext;
    private List<ulong> _lastActiveTeam = new List<ulong>();
    private uint _lastSceneId;

    public bool Enabled { get; set; }

    public AutoEquipService(
      EventManager eventManager,
      Network network,
      AvatarService avatarService,
      StatusService statusService,
      EquipService equipService,
      GameMessageService gameMessageService)
    {
      this._eventManager = eventManager;
      this._network = network;
      this._avatarService = avatarService;
      this._statusService = statusService;
      this._equipService = equipService;
      this._gameMessageService = gameMessageService;
      this._dbContext = new SqliteContext();
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
      int num;
      Task.Run(new Func<Task>(this.Worker), this._cts.Token).ContinueWith((Action<Task>) (t => num = t.IsFaulted ? 1 : 0));
    }

    private async Task Worker()
    {
      while (!this._cts.Token.IsCancellationRequested)
      {
        if (this._statusService.IsServiceOffline || this._statusService.CurrentUid == 0U || this._statusService.CurrentSceneId == 0U || !this.Enabled)
        {
          this._lastActiveTeam.Clear();
          await Task.Delay(1000);
        }
        else
        {
          try
          {
            List<ulong> avatarList = JsonConvert.DeserializeObject<AvatarListDto>(await this._network.GetCurrentTeamAsync())?.AvatarGuids;
            if ((int) this._lastSceneId != (int) this._statusService.CurrentSceneId && avatarList != null)
            {
              await this.OnSceneChanged(this._statusService.CurrentSceneId, avatarList);
              this._lastSceneId = this._statusService.CurrentSceneId;
              continue;
            }
            if (avatarList != null && avatarList.Count > 0)
            {
              avatarList.Sort();
              this._lastActiveTeam.Sort();
              if (!Enumerable.SequenceEqual<ulong>((IEnumerable<ulong>) avatarList, (IEnumerable<ulong>) this._lastActiveTeam))
              {
                await this.OnTeamChanged(avatarList);
                this._lastActiveTeam = avatarList;
              }
            }
            avatarList = (List<ulong>) null;
          }
          catch (Exception ex)
          {
          }
          await Task.Delay(1000);
        }
      }
    }

    private async Task OnTeamChanged(List<ulong> avatarGuids)
    {
      HashSet<ulong> guidSet = new HashSet<ulong>((IEnumerable<ulong>) avatarGuids);
      List<DbRelicProfile> profiles = await ((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) this._dbContext.RelicProfiles, (Expression<Func<DbRelicProfile, bool>>) (p => guidSet.Contains(p.AvatarGuid) && p.AutoEquip)).Include<DbRelicProfile, DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).ThenInclude<DbRelicProfile, DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar))).Include<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>((Expression<Func<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>>) (p => p.TeamContexts))).Include<DbRelicProfile, ICollection<DbRelicItem>>((Expression<Func<DbRelicProfile, ICollection<DbRelicItem>>>) (p => p.RelicItems))).ToListAsync<DbRelicProfile>();
      if (profiles.Count == 0)
      {
        profiles = (List<DbRelicProfile>) null;
      }
      else
      {
        HashSet<uint> currentAvatarIdSet = new HashSet<uint>(Enumerable.Select<DbUserAvatar, uint>((IEnumerable<DbUserAvatar>) Enumerable.ToList<DbUserAvatar>(Enumerable.Where<DbUserAvatar>((IEnumerable<DbUserAvatar>) await Task.WhenAll<DbUserAvatar>(Enumerable.Select<ulong, Task<DbUserAvatar>>((IEnumerable<ulong>) avatarGuids, (Func<ulong, Task<DbUserAvatar>>) (async g => await this._avatarService.GetUserAvatarByGuid(g)))), (Func<DbUserAvatar, bool>) (u => u != null))), (Func<DbUserAvatar, uint>) (a => a.Avatar.AvatarId)));
        List<DbRelicProfile> dbRelicProfileList = new List<DbRelicProfile>();
        foreach (DbRelicProfile dbRelicProfile in profiles)
        {
          if ((!Enumerable.Any<DbRelicProfileTeamContext>((IEnumerable<DbRelicProfileTeamContext>) dbRelicProfile.TeamContexts, (Func<DbRelicProfileTeamContext, bool>) (tc =>
          {
            HashSet<uint> uintSet1 = new HashSet<uint>((IEnumerable<uint>) tc.AvatarIds);
            uintSet1.Add(tc.Profile.UserAvatar.Avatar.AvatarId);
            HashSet<uint> uintSet2 = uintSet1;
            return uintSet2.SetEquals((IEnumerable<uint>) currentAvatarIdSet) && uintSet2.Count == currentAvatarIdSet.Count;
          })) ? 0 : (dbRelicProfile.WithScene.Count == 0 ? 1 : (dbRelicProfile.WithScene.Contains(this._statusService.CurrentSceneId) ? 1 : 0))) != 0)
            dbRelicProfileList.Add(dbRelicProfile);
        }
        Queue<DbRelicProfile> dbRelicProfileQueue = new Queue<DbRelicProfile>((IEnumerable<DbRelicProfile>) dbRelicProfileList);
        while (dbRelicProfileQueue.Count > 1)
        {
          DbRelicProfile left = dbRelicProfileQueue.Dequeue();
          foreach (DbRelicProfile right in dbRelicProfileQueue)
          {
            if (this.ProfileHasConflig(left, right))
            {
              dbRelicProfileQueue.Clear();
              DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 4);
              interpolatedStringHandler.AppendLiteral("[自动配置] ");
              interpolatedStringHandler.AppendFormatted(left.UserAvatar.Avatar.Name);
              interpolatedStringHandler.AppendLiteral("->");
              interpolatedStringHandler.AppendFormatted(left.ProfileName);
              interpolatedStringHandler.AppendLiteral(" 与 ");
              interpolatedStringHandler.AppendFormatted(right.UserAvatar.Avatar.Name);
              interpolatedStringHandler.AppendLiteral("->");
              interpolatedStringHandler.AppendFormatted(right.ProfileName);
              interpolatedStringHandler.AppendLiteral(" 配置冲突");
              string stringAndClear = interpolatedStringHandler.ToStringAndClear();
              this._eventManager.FireEventAsync(EventId.EvtProfileConflict, (object) stringAndClear);
              this._gameMessageService.EnqueueMessage(stringAndClear);
              break;
            }
          }
        }
        if (dbRelicProfileQueue.Count == 0)
        {
          profiles = (List<DbRelicProfile>) null;
        }
        else
        {
          foreach (DbRelicProfile dbRelicProfile in dbRelicProfileList)
          {
            string profileName = dbRelicProfile.ProfileName;
            string avatarName = dbRelicProfile.UserAvatar.Avatar.Name;
            ulong avatarGuid = dbRelicProfile.AvatarGuid;
            foreach (DbRelicItem relicItem in (IEnumerable<DbRelicItem>) dbRelicProfile.RelicItems)
            {
              int num = await this._equipService.WearEquip(avatarGuid, relicItem.Guid) ? 1 : 0;
            }
            GameMessageService gameMessageService = this._gameMessageService;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
            interpolatedStringHandler.AppendLiteral("[自动配置] 已为 [");
            interpolatedStringHandler.AppendFormatted(avatarName);
            interpolatedStringHandler.AppendLiteral("] 装备 [");
            interpolatedStringHandler.AppendFormatted(profileName);
            interpolatedStringHandler.AppendLiteral("] 预设");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            gameMessageService.EnqueueMessage(stringAndClear);
            profileName = (string) null;
            avatarName = (string) null;
          }
          profiles = (List<DbRelicProfile>) null;
        }
      }
    }

    private async Task OnSceneChanged(uint sceneId, List<ulong> avatarGuids)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      AutoEquipService.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new AutoEquipService.\u003C\u003Ec__DisplayClass17_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.sceneId = sceneId;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.guidSet = new HashSet<ulong>((IEnumerable<ulong>) avatarGuids);
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: field reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: reference to a compiler-generated method
      List<DbRelicProfile> list = Enumerable.ToList<DbRelicProfile>(Enumerable.Where<DbRelicProfile>((IEnumerable<DbRelicProfile>) await ((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) Queryable.Where<DbRelicProfile>((IQueryable<DbRelicProfile>) ((IQueryable<DbRelicProfile>) this._dbContext.RelicProfiles).Include<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>((Expression<Func<DbRelicProfile, ICollection<DbRelicProfileTeamContext>>>) (p => p.TeamContexts)), Expression.Lambda<Func<DbRelicProfile, bool>>((Expression) Expression.AndAlso((Expression) Expression.AndAlso(p.AutoEquip, (Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DbRelicProfile.get_TeamContexts))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ICollection<DbRelicProfileTeamContext>.get_Count), __typeref (ICollection<DbRelicProfileTeamContext>))), (Expression) Expression.Constant((object) 0, typeof (int)))), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass170, typeof (AutoEquipService.\u003C\u003Ec__DisplayClass17_0)), FieldInfo.GetFieldFromHandle(__fieldref (AutoEquipService.\u003C\u003Ec__DisplayClass17_0.guidSet))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (HashSet<ulong>.Contains), __typeref (HashSet<ulong>)), new Expression[1]
      {
        (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DbRelicProfile.get_AvatarGuid)))
      })), new ParameterExpression[1]{ parameterExpression })).Include<DbRelicProfile, DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).ThenInclude<DbRelicProfile, DbUserAvatar, DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (ua => ua.Avatar))).Include<DbRelicProfile, ICollection<DbRelicItem>>((Expression<Func<DbRelicProfile, ICollection<DbRelicItem>>>) (dbRelicProfile => dbRelicProfile.RelicItems))).ToListAsync<DbRelicProfile>(), new Func<DbRelicProfile, bool>(cDisplayClass170.\u003COnSceneChanged\u003Eb__0)));
      if (list.Count == 0)
      {
        cDisplayClass170 = (AutoEquipService.\u003C\u003Ec__DisplayClass17_0) null;
      }
      else
      {
        Queue<DbRelicProfile> dbRelicProfileQueue = new Queue<DbRelicProfile>((IEnumerable<DbRelicProfile>) list);
        while (dbRelicProfileQueue.Count > 1)
        {
          DbRelicProfile left = dbRelicProfileQueue.Dequeue();
          foreach (DbRelicProfile right in dbRelicProfileQueue)
          {
            if (this.ProfileHasConflig(left, right))
            {
              dbRelicProfileQueue.Clear();
              DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 4);
              interpolatedStringHandler.AppendLiteral("[自动配置] ");
              interpolatedStringHandler.AppendFormatted(left.UserAvatar.Avatar.Name);
              interpolatedStringHandler.AppendLiteral("->");
              interpolatedStringHandler.AppendFormatted(left.ProfileName);
              interpolatedStringHandler.AppendLiteral(" 与 ");
              interpolatedStringHandler.AppendFormatted(right.UserAvatar.Avatar.Name);
              interpolatedStringHandler.AppendLiteral("->");
              interpolatedStringHandler.AppendFormatted(right.ProfileName);
              interpolatedStringHandler.AppendLiteral(" 配置冲突");
              string stringAndClear = interpolatedStringHandler.ToStringAndClear();
              this._eventManager.FireEventAsync(EventId.EvtProfileConflict, (object) stringAndClear);
              this._gameMessageService.EnqueueMessage(stringAndClear);
              break;
            }
          }
        }
        if (dbRelicProfileQueue.Count == 0)
        {
          cDisplayClass170 = (AutoEquipService.\u003C\u003Ec__DisplayClass17_0) null;
        }
        else
        {
          foreach (DbRelicProfile dbRelicProfile in list)
          {
            string profileName = dbRelicProfile.ProfileName;
            string avatarName = dbRelicProfile.UserAvatar.Avatar.Name;
            ulong avatarGuid = dbRelicProfile.AvatarGuid;
            foreach (DbRelicItem relicItem in (IEnumerable<DbRelicItem>) dbRelicProfile.RelicItems)
            {
              int num = await this._equipService.WearEquip(avatarGuid, relicItem.Guid) ? 1 : 0;
            }
            GameMessageService gameMessageService = this._gameMessageService;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
            interpolatedStringHandler.AppendLiteral("[自动配置] 已为 [");
            interpolatedStringHandler.AppendFormatted(avatarName);
            interpolatedStringHandler.AppendLiteral("] 装备 [");
            interpolatedStringHandler.AppendFormatted(profileName);
            interpolatedStringHandler.AppendLiteral("] 预设");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            gameMessageService.EnqueueMessage(stringAndClear);
            profileName = (string) null;
            avatarName = (string) null;
          }
          cDisplayClass170 = (AutoEquipService.\u003C\u003Ec__DisplayClass17_0) null;
        }
      }
    }

    private bool ProfileHasConflig(DbRelicProfile left, DbRelicProfile right)
    {
      return new HashSet<ulong>(Enumerable.Select<DbRelicItem, ulong>((IEnumerable<DbRelicItem>) left.RelicItems, (Func<DbRelicItem, ulong>) (r => r.Guid))).Overlaps((IEnumerable<ulong>) new HashSet<ulong>(Enumerable.Select<DbRelicItem, ulong>((IEnumerable<DbRelicItem>) right.RelicItems, (Func<DbRelicItem, ulong>) (r => r.Guid))));
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
