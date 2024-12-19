using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;

namespace RelicService.Service;

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

	public AutoEquipService(EventManager eventManager, Network network, AvatarService avatarService, StatusService statusService, EquipService equipService, GameMessageService gameMessageService)
	{
		_eventManager = eventManager;
		_network = network;
		_avatarService = avatarService;
		_statusService = statusService;
		_equipService = equipService;
		_gameMessageService = gameMessageService;
		_dbContext = new SqliteContext();
		_eventManager.OnShutdown += OnShutdown;
		Task.Run((Func<Task?>)Worker, _cts.Token).ContinueWith(delegate(Task t)
		{
			_ = t.IsFaulted;
		});
	}

	private async Task Worker()
	{
		while (!_cts.Token.IsCancellationRequested)
		{
			if (_statusService.IsServiceOffline || _statusService.CurrentUid == 0 || _statusService.CurrentSceneId == 0 || !Enabled)
			{
				_lastActiveTeam.Clear();
				await Task.Delay(1000);
				continue;
			}
			try
			{
				List<ulong> avatarList = JsonConvert.DeserializeObject<AvatarListDto>(await _network.GetCurrentTeamAsync())?.AvatarGuids;
				if (_lastSceneId != _statusService.CurrentSceneId && avatarList != null)
				{
					await OnSceneChanged(_statusService.CurrentSceneId, avatarList);
					_lastSceneId = _statusService.CurrentSceneId;
					continue;
				}
				if (avatarList != null && avatarList.Count > 0)
				{
					avatarList.Sort();
					_lastActiveTeam.Sort();
					if (!avatarList.SequenceEqual(_lastActiveTeam))
					{
						await OnTeamChanged(avatarList);
						_lastActiveTeam = avatarList;
					}
				}
			}
			catch (Exception)
			{
			}
			await Task.Delay(1000);
		}
	}

	private async Task OnTeamChanged(List<ulong> avatarGuids)
	{
		HashSet<ulong> guidSet = new HashSet<ulong>(avatarGuids);
		List<DbRelicProfile> profiles = await _dbContext.RelicProfiles.Where((DbRelicProfile p) => guidSet.Contains(p.AvatarGuid) && p.AutoEquip).Include((DbRelicProfile p) => p.UserAvatar).ThenInclude((DbUserAvatar ua) => ua.Avatar)
			.Include((DbRelicProfile p) => p.TeamContexts)
			.Include((DbRelicProfile p) => p.RelicItems)
			.ToListAsync();
		if (profiles.Count == 0)
		{
			return;
		}
		List<DbUserAvatar> source = (await Task.WhenAll(avatarGuids.Select(async (ulong g) => await _avatarService.GetUserAvatarByGuid(g)))).Where((DbUserAvatar u) => u != null).ToList();
		HashSet<uint> currentAvatarIdSet = new HashSet<uint>(source.Select((DbUserAvatar a) => a.Avatar.AvatarId));
		List<DbRelicProfile> list = new List<DbRelicProfile>();
		foreach (DbRelicProfile item in profiles)
		{
			if (item.TeamContexts.Any(delegate(DbRelicProfileTeamContext tc)
			{
				HashSet<uint> hashSet = new HashSet<uint>(tc.AvatarIds) { tc.Profile.UserAvatar.Avatar.AvatarId };
				return hashSet.SetEquals(currentAvatarIdSet) && hashSet.Count == currentAvatarIdSet.Count;
			}) && (item.WithScene.Count == 0 || item.WithScene.Contains(_statusService.CurrentSceneId)))
			{
				list.Add(item);
			}
		}
		Queue<DbRelicProfile> queue = new Queue<DbRelicProfile>(list);
		while (queue.Count > 1)
		{
			DbRelicProfile dbRelicProfile = queue.Dequeue();
			foreach (DbRelicProfile item2 in queue)
			{
				if (ProfileHasConflig(dbRelicProfile, item2))
				{
					queue.Clear();
					string text = $"[自动配置] {dbRelicProfile.UserAvatar.Avatar.Name}->{dbRelicProfile.ProfileName} 与 {item2.UserAvatar.Avatar.Name}->{item2.ProfileName} 配置冲突";
					_eventManager.FireEventAsync(EventId.EvtProfileConflict, text);
					_gameMessageService.EnqueueMessage(text);
					break;
				}
			}
		}
		if (queue.Count == 0)
		{
			return;
		}
		foreach (DbRelicProfile item3 in list)
		{
			string profileName = item3.ProfileName;
			string avatarName = item3.UserAvatar.Avatar.Name;
			ulong avatarGuid = item3.AvatarGuid;
			foreach (DbRelicItem relicItem in item3.RelicItems)
			{
				await _equipService.WearEquip(avatarGuid, relicItem.Guid);
			}
			_gameMessageService.EnqueueMessage($"[自动配置] 已为 [{avatarName}] 装备 [{profileName}] 预设");
		}
	}

	private async Task OnSceneChanged(uint sceneId, List<ulong> avatarGuids)
	{
		HashSet<ulong> guidSet = new HashSet<ulong>(avatarGuids);
		List<DbRelicProfile> list = (await (from p in _dbContext.RelicProfiles.Include((DbRelicProfile p) => p.TeamContexts)
			where p.AutoEquip && p.TeamContexts.Count == 0 && guidSet.Contains(p.AvatarGuid)
			select p).Include((DbRelicProfile p) => p.UserAvatar).ThenInclude((DbUserAvatar ua) => ua.Avatar).Include((DbRelicProfile dbRelicProfile) => dbRelicProfile.RelicItems)
			.ToListAsync()).Where((DbRelicProfile p) => p.WithScene.Contains(sceneId)).ToList();
		if (list.Count == 0)
		{
			return;
		}
		Queue<DbRelicProfile> queue = new Queue<DbRelicProfile>(list);
		while (queue.Count > 1)
		{
			DbRelicProfile dbRelicProfile2 = queue.Dequeue();
			foreach (DbRelicProfile item in queue)
			{
				if (ProfileHasConflig(dbRelicProfile2, item))
				{
					queue.Clear();
					string text = $"[自动配置] {dbRelicProfile2.UserAvatar.Avatar.Name}->{dbRelicProfile2.ProfileName} 与 {item.UserAvatar.Avatar.Name}->{item.ProfileName} 配置冲突";
					_eventManager.FireEventAsync(EventId.EvtProfileConflict, text);
					_gameMessageService.EnqueueMessage(text);
					break;
				}
			}
		}
		if (queue.Count == 0)
		{
			return;
		}
		foreach (DbRelicProfile item2 in list)
		{
			string profileName = item2.ProfileName;
			string avatarName = item2.UserAvatar.Avatar.Name;
			ulong avatarGuid = item2.AvatarGuid;
			foreach (DbRelicItem relicItem in item2.RelicItems)
			{
				await _equipService.WearEquip(avatarGuid, relicItem.Guid);
			}
			_gameMessageService.EnqueueMessage($"[自动配置] 已为 [{avatarName}] 装备 [{profileName}] 预设");
		}
	}

	private bool ProfileHasConflig(DbRelicProfile left, DbRelicProfile right)
	{
		HashSet<ulong> hashSet = new HashSet<ulong>(left.RelicItems.Select((DbRelicItem r) => r.Guid));
		HashSet<ulong> other = new HashSet<ulong>(right.RelicItems.Select((DbRelicItem r) => r.Guid));
		return hashSet.Overlaps(other);
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
