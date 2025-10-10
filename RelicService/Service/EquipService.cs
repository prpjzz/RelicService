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
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Service
{
    internal class EquipService
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly EventManager _eventManager;
        private readonly Network _network;
        private StatusService _statusService;
        private readonly SqliteContext _dbContext;
        private uint _fetchCurrent = 0;
        private uint _fetchTotal = 0;
        private FetchType _fetchType = FetchType.None;

        private readonly object _lock = new();
        private readonly HashSet<uint> _pendingRelics = new();

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

        public EquipService(
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

        public async Task<bool> WearEquip(ulong avatarGuid, ulong relicGuid)
        {
            try
            {
                await _network.UpdateAvatarEquipAsync(avatarGuid, relicGuid);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<List<RelicDataDto>?> UpdateAvatarEquipFromGame(ulong avatarGuid)
        {
            try
            {
                _fetchType = FetchType.RelicMetadata;
                FetchTotal = 0;

                var relicList = await FetchAvatarEquipFromGame(avatarGuid);

                await UpdateRelicData(relicList);
                await _dbContext.SaveChangesAsync();

                return relicList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                FinishFetch();
            }
        }

        public async Task<List<DbRelicItem>> UpdateAndSaveAvatarEquip(ulong avatarGuid)
        {
            try
            {
                _fetchType = FetchType.RelicMetadata;
                FetchTotal = 0;

                var relicList = await FetchAvatarEquipFromGame(avatarGuid);

                await UpdateRelicData(relicList);
                var result = await UpdateRelicDetails(relicList);

                await _dbContext.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new();
            }
            finally
            {
                FinishFetch();
            }
        }

        private async Task<List<RelicDataDto>> FetchAvatarEquipFromGame(ulong guid)
        {
            try
            {
                var rawResult = await _network.GetAvatarRelicsAsync(guid);
                var relicListDto = JsonConvert.DeserializeObject<List<RelicDataDto>>(rawResult);
                if (relicListDto == null)
                    throw new Exception($"failed to deserialize: {rawResult}");

                return relicListDto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new();
            }
        }

        private async Task UpdateRelicData(List<RelicDataDto> relicList)
        {

            try
            {
                _pendingRelics.Clear();

                _fetchType = FetchType.RelicMetadata;
                FetchTotal = (uint)relicList.Count;

                List<Task> tasks = new();
                relicList.ForEach(relic => tasks.Add(UpdateRelicMetadata(relic)));
                //foreach (var relic in relicList)
                //{
                //    await UpdateRelicMetadata(relic);
                //}

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FinishFetch();
            }

        }

        private async Task UpdateRelicMetadata(RelicDataDto relic)
        {
            if (_pendingRelics.Contains(relic.ItemId))
                return;

            var dbRelic = await _dbContext.Relics.FindAsync(relic.ItemId);
            if (dbRelic != null)
                return;

            dbRelic = new()
            {
                ItemId = relic.ItemId,
                TextId = relic.NameTextId,
                Name = await GetEquipName(relic.NameTextId),
                IconName = relic.IconName,
                IconBase64 = await GetEquipImage(relic.IconName)
            };

            _dbContext.Relics.Add(dbRelic);
            _pendingRelics.Add(relic.ItemId);

            FetchCurrent++;
        }

        private async Task<List<DbRelicItem>> UpdateRelicDetails(List<RelicDataDto> relicDatas)
        {
            List<DbRelicItem> dbRelicItems = new();
            foreach (var relic in relicDatas)
            {
                var dbRelicItem = await UpdateRelicDetail(relic);
                dbRelicItems.Add(dbRelicItem);
            }

            return dbRelicItems;
        }

        private async Task<DbRelicItem> UpdateRelicDetail(RelicDataDto relic)
        {
            var dbRelicItem = await _dbContext.RelicItems
                .Include(r => r.Affixes)
                .Where(r => r.Guid == relic.Guid)
                .FirstOrDefaultAsync();


            if (dbRelicItem != null)
                return dbRelicItem;

            var relicMetadata = await _dbContext.Relics.FindAsync(relic.ItemId);
            if (relicMetadata == null)
                throw new Exception($"relic metadata not found: {relic.ItemId}");

            dbRelicItem = new()
            {
                Guid = relic.Guid,
                Level = relic.Level,
                MainPropId = relic.MainPropId,
                RankLevel = relic.RankLevel,
                EquipType = relic.EquipType,
                MainPropType = relic.MainPropType,
                MainPropValue = relic.MainPropValue,
                Relic = relicMetadata
            };

            foreach (var prop in relic.AppendProp)
            {
                var affix = new DbRelicAffix
                {
                    PropType = prop.PropType,
                    PropValue = prop.PropValue,
                    Relic = dbRelicItem
                };
                dbRelicItem.Affixes.Add(affix);
            }

            _dbContext.RelicItems.Add(dbRelicItem);

            return dbRelicItem;
        }


        private async Task<string> GetEquipName(uint nameTextId)
        {
            var result = await _network.GetTextAsync(nameTextId);
            return result;
        }

        private async Task<string> GetEquipImage(string imageName)
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
