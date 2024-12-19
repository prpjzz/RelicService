using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RelicService.Data.Config;
using RelicService.Data.Database;
using RelicService.Data.Dto;
using RelicService.Data.Event;
using RelicService.Tools;

namespace RelicService.Service;

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
			bool num = Monitor.TryEnter(_lock);
			if (num)
			{
				Monitor.Exit(_lock);
			}
			return !num;
		}
		private set
		{
		}
	}

	public uint FetchCurrent
	{
		get
		{
			return _fetchCurrent;
		}
		private set
		{
			Interlocked.Exchange(ref _fetchCurrent, value);
			_eventManager.FireEventAsync(EventId.EvtFetchProgress, new FetchProgressEvent(_fetchType, value, _fetchTotal));
		}
	}

	public uint FetchTotal
	{
		get
		{
			return _fetchTotal;
		}
		private set
		{
			Interlocked.Exchange(ref _fetchTotal, value);
			_eventManager.FireEventAsync(EventId.EvtFetchProgress, new FetchProgressEvent(_fetchType, _fetchCurrent, value));
		}
	}

	public EquipService(EventManager eventManager, Network network, StatusService statusService, SqliteContext dbContext)
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
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	public async Task<List<RelicDataDto>> UpdateAvatarEquipFromGame(ulong avatarGuid)
	{
		_ = 2;
		try
		{
			_fetchType = FetchType.RelicMetadata;
			FetchTotal = 0u;
			List<RelicDataDto> relicList = await FetchAvatarEquipFromGame(avatarGuid);
			await UpdateRelicData(relicList);
			await _dbContext.SaveChangesAsync();
			return relicList;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return new List<RelicDataDto>();
		}
		finally
		{
			FinishFetch();
		}
	}

	public async Task<List<DbRelicItem>> UpdateAndSaveAvatarEquip(ulong avatarGuid)
	{
		_ = 3;
		try
		{
			_fetchType = FetchType.RelicMetadata;
			FetchTotal = 0u;
			List<RelicDataDto> relicList = await FetchAvatarEquipFromGame(avatarGuid);
			await UpdateRelicData(relicList);
			List<DbRelicItem> result = await UpdateRelicDetails(relicList);
			await _dbContext.SaveChangesAsync();
			return result;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return new List<DbRelicItem>();
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
			string text = await _network.GetAvatarRelicsAsync(guid);
			return JsonConvert.DeserializeObject<List<RelicDataDto>>(text) ?? throw new Exception("failed to deserialize: " + text);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return new List<RelicDataDto>();
		}
	}

	private async Task UpdateRelicData(List<RelicDataDto> relicList)
	{
		try
		{
			_pendingRelics.Clear();
			_fetchType = FetchType.RelicMetadata;
			FetchTotal = (uint)relicList.Count;
			List<Task> tasks = new List<Task>();
			relicList.ForEach(delegate(RelicDataDto relic)
			{
				tasks.Add(UpdateRelicMetadata(relic));
			});
			await Task.WhenAll(tasks);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		finally
		{
			FinishFetch();
		}
	}

	private async Task UpdateRelicMetadata(RelicDataDto relic)
	{
		if (!_pendingRelics.Contains(relic.ItemId) && await _dbContext.Relics.FindAsync(relic.ItemId) == null)
		{
			DbRelic dbRelic = new DbRelic
			{
				ItemId = relic.ItemId,
				TextId = relic.NameTextId
			};
			DbRelic dbRelic2 = dbRelic;
			dbRelic2.Name = await GetEquipName(relic.NameTextId);
			dbRelic.IconName = relic.IconName;
			DbRelic dbRelic3 = dbRelic;
			dbRelic3.IconBase64 = await GetEquipImage(relic.IconName);
			_dbContext.Relics.Add(dbRelic);
			_pendingRelics.Add(relic.ItemId);
			FetchCurrent++;
		}
	}

	private async Task<List<DbRelicItem>> UpdateRelicDetails(List<RelicDataDto> relicDatas)
	{
		List<DbRelicItem> dbRelicItems = new List<DbRelicItem>();
		foreach (RelicDataDto relicData in relicDatas)
		{
			dbRelicItems.Add(await UpdateRelicDetail(relicData));
		}
		return dbRelicItems;
	}

	private async Task<DbRelicItem> UpdateRelicDetail(RelicDataDto relic)
	{
		DbRelicItem dbRelicItem = await (from r in _dbContext.RelicItems.Include((DbRelicItem r) => r.Affixes)
			where r.Guid == relic.Guid
			select r).FirstOrDefaultAsync();
		if (dbRelicItem != null)
		{
			return dbRelicItem;
		}
		DbRelic dbRelic = await _dbContext.Relics.FindAsync(relic.ItemId);
		if (dbRelic == null)
		{
			throw new Exception($"relic metadata not found: {relic.ItemId}");
		}
		dbRelicItem = new DbRelicItem
		{
			Guid = relic.Guid,
			Level = relic.Level,
			MainPropId = relic.MainPropId,
			RankLevel = relic.RankLevel,
			EquipType = relic.EquipType,
			MainPropType = relic.MainPropType,
			MainPropValue = relic.MainPropValue,
			Relic = dbRelic
		};
		foreach (RelicAffixDto item2 in relic.AppendProp)
		{
			DbRelicAffix item = new DbRelicAffix
			{
				PropType = item2.PropType,
				PropValue = item2.PropValue,
				Relic = dbRelicItem
			};
			dbRelicItem.Affixes.Add(item);
		}
		_dbContext.RelicItems.Add(dbRelicItem);
		return dbRelicItem;
	}

	private async Task<string> GetEquipName(uint nameTextId)
	{
		return await _network.GetTextAsync(nameTextId);
	}

	private async Task<string> GetEquipImage(string imageName)
	{
		return await _network.GetItemImageAsync(imageName);
	}

	private void FinishFetch()
	{
		ResetFetchState();
		_fetchType = FetchType.None;
		_eventManager.FireEventAsync(EventId.EvtFetchProgress, new FetchProgressEvent(FetchType.None, 0u, 0u));
	}

	private void ResetFetchState()
	{
		_fetchType = FetchType.None;
		FetchCurrent = 0u;
		FetchTotal = 0u;
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
