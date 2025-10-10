using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Service
{
    internal class AutoEquipService
    {
        private readonly CancellationTokenSource _cts = new();
        private EventManager _eventManager;
        private Network _network;
        private AvatarService _avatarService;
        private StatusService _statusService;
        private EquipService _equipService;
        private GameMessageService _gameMessageService;
        private SqliteContext _dbContext;

        private List<ulong> _lastActiveTeam = new();
        private uint _lastSceneId = 0;

        public bool Enabled { get; set; } = false;

        public AutoEquipService(
            EventManager eventManager,
            Network network,
            AvatarService avatarService,
            StatusService statusService,
            EquipService equipService,
            GameMessageService gameMessageService
            )
        {
            _eventManager = eventManager;
            _network = network;
            _avatarService = avatarService;
            _statusService = statusService;
            _equipService = equipService;
            _gameMessageService = gameMessageService;
            _dbContext = new SqliteContext();

            _eventManager.OnShutdown += OnShutdown;

            _ = Task.Run(Worker, _cts.Token)
                .ContinueWith(t =>
                {

                    if (t.IsFaulted)
                    {
                        Debug.WriteLine(t.Exception?.InnerException?.StackTrace);
                    }

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

                    var rawResult = await _network.GetCurrentTeamAsync();
                    var avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(rawResult);
                    var avatarList = avatarListDto?.AvatarGuids;

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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                await Task.Delay(1000);

            }

        }

        private async Task OnTeamChanged(List<ulong> avatarGuids)
        {
            var guidSet = new HashSet<ulong>(avatarGuids);

            // first filter out the profiles that are not in the team
            var profiles = await _dbContext.RelicProfiles
                .Where(p => guidSet.Contains(p.AvatarGuid) && p.AutoEquip)
                .Include(p => p.UserAvatar)
                .ThenInclude(ua => ua.Avatar)
                .Include(p => p.TeamContexts)
                .Include(p => p.RelicItems)
                .ToListAsync();

            if (profiles.Count == 0)
                return;

            var currentTeamAvatars = (await Task.WhenAll(avatarGuids
                .Select(async g => await _avatarService.GetUserAvatarByGuid(g))))
                .Where(u => u != null)
                .ToList();

            var currentAvatarIdSet = new HashSet<uint>(currentTeamAvatars.Select(a => a.Avatar.AvatarId));

            var candidateProfiles = new List<DbRelicProfile>();
            foreach (var profile in profiles)
            {
                // Step 2: Match profiles with team contexts
                bool hasTeamMatch = profile.TeamContexts.Any(tc =>
                {
                    var avatarIdSet = new HashSet<uint>(tc.AvatarIds)
                    {
                        tc.Profile.UserAvatar.Avatar.AvatarId // add self to set for test
                    };
                    return avatarIdSet.SetEquals(currentAvatarIdSet) && avatarIdSet.Count == currentAvatarIdSet.Count;
                });

                bool condition = hasTeamMatch &&
                    (profile.WithScene.Count == 0 || profile.WithScene.Contains(_statusService.CurrentSceneId));

                if (condition)
                    candidateProfiles.Add(profile);

            }

            // check for conflicts
            var profileQueue = new Queue<DbRelicProfile>(candidateProfiles);
            while (profileQueue.Count > 1)
            {
                var profile = profileQueue.Dequeue();

                foreach (var otherProfile in profileQueue)
                {
                    if (ProfileHasConflig(profile, otherProfile))
                    {
                        profileQueue.Clear();

                        var message = $"[自动配置] {profile.UserAvatar.Avatar.Name}->{profile.ProfileName} 与 {otherProfile.UserAvatar.Avatar.Name}->{otherProfile.ProfileName} 配置冲突";
                        _eventManager.FireEventAsync(Data.Event.EventId.EvtProfileConflict, message);
                        _gameMessageService.EnqueueMessage(message);
                        // notify
                        break;
                    }
                }

            }

            // indicates conflict, do not proceed
            // there will be 1 profile left in the queue if no conflict
            if (profileQueue.Count == 0)
                return;

            foreach (var profile in candidateProfiles)
            {
                var profileName = profile.ProfileName;
                var avatarName = profile.UserAvatar.Avatar.Name;
                var avatarGuid = profile.AvatarGuid;

                foreach (var relicItem in profile.RelicItems)
                {
                    await _equipService.WearEquip(avatarGuid, relicItem.Guid);
                }

                _gameMessageService.EnqueueMessage($"[自动配置] 已为 [{avatarName}] 装备 [{profileName}] 预设");
            }

        }

        private async Task OnSceneChanged(uint sceneId, List<ulong> avatarGuids)
        {
            var guidSet = new HashSet<ulong>(avatarGuids);

            var profiles = await _dbContext.RelicProfiles
                .Include(p => p.TeamContexts)
                .Where(p => p.AutoEquip && p.TeamContexts.Count == 0 && guidSet.Contains(p.AvatarGuid))
                .Include(p => p.UserAvatar)
                .ThenInclude(ua => ua.Avatar)
                .Include(dbRelicProfile => dbRelicProfile.RelicItems)
                .ToListAsync();

            profiles = profiles
                .Where(p => p.WithScene.Contains(sceneId))
                .ToList();

            if (profiles.Count == 0)
                return;

            // check for conflicts
            var profileQueue = new Queue<DbRelicProfile>(profiles);
            while (profileQueue.Count > 1)
            {
                var profile = profileQueue.Dequeue();

                foreach (var otherProfile in profileQueue)
                {
                    if (ProfileHasConflig(profile, otherProfile))
                    {
                        profileQueue.Clear();

                        var message = $"[自动配置] {profile.UserAvatar.Avatar.Name}->{profile.ProfileName} 与 {otherProfile.UserAvatar.Avatar.Name}->{otherProfile.ProfileName} 配置冲突";
                        _eventManager.FireEventAsync(Data.Event.EventId.EvtProfileConflict, message);
                        _gameMessageService.EnqueueMessage(message);
                        // notify
                        break;
                    }
                }

            }

            // indicates conflict, do not proceed
            // there will be 1 profile left in the queue if no conflict
            if (profileQueue.Count == 0)
                return;

            // give some time for the game to finish loading into new scene
            //await Task.Delay(5000);

            foreach (var profile in profiles)
            {
                var profileName = profile.ProfileName;
                var avatarName = profile.UserAvatar.Avatar.Name;
                var avatarGuid = profile.AvatarGuid;

                foreach (var relicItem in profile.RelicItems)
                {
                    await _equipService.WearEquip(avatarGuid, relicItem.Guid);
                }

                _gameMessageService.EnqueueMessage($"[自动配置] 已为 [{avatarName}] 装备 [{profileName}] 预设");
            }
        }

        private bool ProfileHasConflig(DbRelicProfile left, DbRelicProfile right)
        {
            var leftRelicGuidSet = new HashSet<ulong>(left.RelicItems.Select(r => r.Guid));
            var rightRelicGuidSet = new HashSet<ulong>(right.RelicItems.Select(r => r.Guid));

            return leftRelicGuidSet.Overlaps(rightRelicGuidSet);
        }

        private void OnShutdown(object? sender, EventArgs e) => _cts.Cancel();

    }
}
