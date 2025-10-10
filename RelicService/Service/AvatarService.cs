using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace RelicService.Service
{
    internal class AvatarService
    {

        private readonly CancellationTokenSource _cts = new();
        private readonly EventManager _eventManager;
        private readonly Network _network;
        private readonly StatusService _statusService;
        private readonly SqliteContext _dbContext;
        private uint _fetchCurrent = 0;
        private uint _fetchTotal = 0;
        private FetchType _fetchType = FetchType.None;

        private readonly object _lock = new();

        public bool IsBusy
        {
            get
            {
                var result = Monitor.TryEnter(_lock);
                if (result)
                    Monitor.Exit(_lock);
                return !result;
            }

            private set { }
        }

        public uint FetchCurrent
        {
            get { return _fetchCurrent; }
            private set
            {
                Interlocked.Exchange(ref _fetchCurrent, value);
                _eventManager.FireEventAsync(EventId.EvtFetchProgress, 
                    new FetchProgressEvent(type: _fetchType, current: value, total: _fetchTotal));
            }
        }
        public uint FetchTotal
        {
            get { return _fetchTotal; }
            private set
            {
                Interlocked.Exchange(ref _fetchTotal, value);
                _eventManager.FireEventAsync(EventId.EvtFetchProgress, 
                    new FetchProgressEvent(type: _fetchType, current: _fetchCurrent, total: value));
            }
        }

        public AvatarService(
            EventManager eventManager,
            Network network,
            StatusService statusService,
            SqliteContext dbContext
            )
        {
            _eventManager = eventManager;
            _network = network;
            _statusService = statusService;
            _dbContext = dbContext;

            _eventManager.OnShutdown += OnShutdown;
        }

        public async Task<DbAvatar?> GetAvatarMetadata(uint avatarId)
        {
            var avatar = await _dbContext.Avatars.FindAsync(avatarId);
            return avatar;
        }

        public async Task<DbUserAvatar?> GetUserAvatarByGuid(ulong avatarGuid)
        {
            var avatar = await _dbContext.UserAvatars
                .Include(ua => ua.Avatar)
                .Where(ua => ua.Guid == avatarGuid)
                .FirstOrDefaultAsync(_cts.Token);
            return avatar;
        }

        public async Task<List<DbUserAvatar>?> GetUserAvatars(uint uid)
        {
            var avatars = await _dbContext.UserAvatars
                .Include(x => x.Avatar)
                .Where(x => x.UserUid == uid)
                .ToListAsync(_cts.Token);

            return avatars;
        }

        public async Task<List<DbUserAvatar>?> GetCurrentTeam()
        {
            try
            {
                var rawResult = await _network.GetCurrentTeamAsync();
                var avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(rawResult);
                if (avatarListDto == null || avatarListDto.AvatarGuids == null)
                    return null;

                List<DbUserAvatar> dbAvatars = new();
                foreach (var guid in avatarListDto.AvatarGuids)
                {
                    var dbAvatar = await _dbContext.UserAvatars.FindAsync(guid);
                    if (dbAvatar != null)
                        dbAvatars.Add(dbAvatar);
                }

                return dbAvatars;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting current team: {ex.StackTrace}");
                return null;
            }
        }


        public async Task UpdateTeamFromGame()
        {

            try
            {
                Monitor.Enter(_lock);
                var rawResult = await _network.GetCurrentTeamAsync();
                var avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(rawResult);
                if (avatarListDto == null || avatarListDto.AvatarGuids == null)
                    throw new Exception($"failed to deserialize: {rawResult}");

                Debug.WriteLine($"Got {avatarListDto.AvatarGuids.Count} avatars");

                await UpdateDataAndResources(avatarListDto.AvatarGuids);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating team data: {ex.StackTrace}");
                MessageBox.Show($"Error updating team data: {ex.Message}", @"Error");
            }
            finally
            {
                FinishFetch();
                Monitor.Exit(_lock);
            }

        }

        public async Task UpdateAllAvatarFromGame()
        {

            try
            {
                Monitor.Enter(_lock);
                var rawResult = await _network.GetAllAvatarsAsync();
                var avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(rawResult);
                if (avatarListDto == null || avatarListDto.AvatarGuids == null)
                    throw new Exception($"failed to deserialize: {rawResult}");

                Debug.WriteLine($"Got {avatarListDto.AvatarGuids.Count} avatars");

                await UpdateDataAndResources(avatarListDto.AvatarGuids);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating avatar data: {ex.StackTrace} {ex.InnerException?.Message ?? ex.Message}");
                MessageBox.Show($@"Error updating avatar data: {ex.Message}", @"Error");
            }
            finally
            {
                FinishFetch();
                Monitor.Exit(_lock);
            }

        }

        private async Task UpdateDataAndResources(List<ulong> avatarGuidList)
        {
            List<AvatarDataDto> avatarDataList = await FetchAvatarDataFromGame(avatarGuidList);
            await UpdateAllAvatarData(avatarDataList);
            await _dbContext.SaveChangesAsync(_cts.Token);
        }

        private async Task<List<AvatarDataDto>> FetchAvatarDataFromGame(List<ulong> avatarGuidList)
        {
            ResetFetchState();
            _fetchType = FetchType.AvatarMetadata;
            FetchTotal = (uint)avatarGuidList.Count;

            List<Exception> exceptions = new();
            List<Task<AvatarDataDto?>> tasks = new();
            avatarGuidList.ForEach(avatarGuid =>
                tasks.Add(
                    FetchAvatarDataFromGame(avatarGuid)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            exceptions.Add(t.Exception);
                        }
                        return t.Result;
                    })
                )
            );

            if (exceptions.Count > 0)
                throw exceptions[0];

            var results = await Task.WhenAll(tasks);

            return results
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();
        }

        private async Task<AvatarDataDto?> FetchAvatarDataFromGame(ulong avatarGuid)
        {
            var rawResult = await _network.GetAvatarInfoAsync(avatarGuid);
            var avatarData = JsonConvert.DeserializeObject<AvatarDataDto>(rawResult);
            if (avatarData == null)
                throw new Exception($"failed to deserialize: {rawResult}");

            FetchCurrent++;

            return avatarData;
        }

        private async Task UpdateAllAvatarData(List<AvatarDataDto> avatarDataList)
        {
            ResetFetchState();
            _fetchType = FetchType.AvatarResource;
            FetchTotal = (uint)avatarDataList.Count;

            foreach (var avatarData in avatarDataList)
            {
                await UpdateAvatarData(avatarData);
                FetchCurrent++;
            }
        }

        private async Task UpdateAvatarData(AvatarDataDto avatarData)
        {
            // check if avatar metadata exists
            var dbAvatar = await _dbContext.Avatars.FindAsync(avatarData.AvatarId);
            if (dbAvatar == null)
            {
                // create new avatar metadata
                dbAvatar = new DbAvatar
                {
                    AvatarId = avatarData.AvatarId,
                    Name = await GetAvatarName(avatarData.NameTextId),
                    TextId = avatarData.NameTextId,
                    IconName = avatarData.IconName,
                    IconBase64 = await GetAvatarImage(avatarData.IconName)
                };


                _dbContext.Avatars.Add(dbAvatar);
            }
            else
            {
                _dbContext.Avatars.Attach(dbAvatar);
            }


            await UpdateUserAvatarData(dbAvatar, avatarData);
        }

        private async Task UpdateUserAvatarData(DbAvatar dbAvatar, AvatarDataDto avatarData)
        {
            var uid = _statusService.CurrentUid;

            var dbUserAvatar = await _dbContext.UserAvatars.FindAsync(avatarData.Guid);
            if (dbUserAvatar == null)
            {

                // create new user avatar metadata
                dbUserAvatar = new DbUserAvatar
                {
                    Guid = avatarData.Guid,
                    AvatarId = dbAvatar.AvatarId,
                    UserUid = uid
                };


                _dbContext.UserAvatars.Add(dbUserAvatar);
            }

        }

        private async Task<string> GetAvatarName(uint nameTextId)
        {
            var result = await _network.GetTextAsync(nameTextId);
            return result;
        }

        private async Task<string> GetAvatarImage(string imageName)
        {
            var result = await _network.GetItemImageAsync(imageName);
            return result;
        }

        private void FinishFetch()
        {
            ResetFetchState();
            _fetchType = FetchType.None;
            _eventManager.FireEventAsync(EventId.EvtFetchProgress,
                new FetchProgressEvent(type: FetchType.None, current: 0, total: 0));
        }

        private void ResetFetchState()
        {
            _fetchType = FetchType.None;
            FetchCurrent = 0;
            FetchTotal = 0;
        }

        private void OnShutdown(object? sender, EventArgs e) => _cts.Cancel();
        

    }
}
