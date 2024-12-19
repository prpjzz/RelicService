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

	public AvatarService(EventManager eventManager, Network network, StatusService statusService, SqliteContext dbContext)
	{
		_eventManager = eventManager;
		_network = network;
		_statusService = statusService;
		_dbContext = dbContext;
		_eventManager.OnShutdown += OnShutdown;
	}

	public async Task<DbAvatar?> GetAvatarMetadata(uint avatarId)
	{
		return await _dbContext.Avatars.FindAsync(avatarId);
	}

	public async Task<DbUserAvatar?> GetUserAvatarByGuid(ulong avatarGuid)
	{
		return await (from ua in _dbContext.UserAvatars.Include((DbUserAvatar ua) => ua.Avatar)
			where ua.Guid == avatarGuid
			select ua).FirstOrDefaultAsync(_cts.Token);
	}

	public async Task<List<DbUserAvatar>?> GetUserAvatars(uint uid)
	{
		return await (from x in _dbContext.UserAvatars.Include((DbUserAvatar x) => x.Avatar)
			where x.UserUid == uid
			select x).ToListAsync(_cts.Token);
	}

	public async Task<List<DbUserAvatar>?> GetCurrentTeam()
	{
		_ = 1;
		try
		{
			AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(await _network.GetCurrentTeamAsync());
			if (avatarListDto == null || avatarListDto.AvatarGuids == null)
			{
				return null;
			}
			List<DbUserAvatar> dbAvatars = new List<DbUserAvatar>();
			foreach (ulong avatarGuid in avatarListDto.AvatarGuids)
			{
				DbUserAvatar dbUserAvatar = await _dbContext.UserAvatars.FindAsync(avatarGuid);
				if (dbUserAvatar != null)
				{
					dbAvatars.Add(dbUserAvatar);
				}
			}
			return dbAvatars;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public async Task UpdateTeamFromGame()
	{
		_ = 1;
		try
		{
			Monitor.Enter(_lock);
			string text = await _network.GetCurrentTeamAsync();
			AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(text);
			if (avatarListDto == null || avatarListDto.AvatarGuids == null)
			{
				throw new Exception("failed to deserialize: " + text);
			}
			await UpdateDataAndResources(avatarListDto.AvatarGuids);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error updating team data: " + ex.Message, "Error");
		}
		finally
		{
			FinishFetch();
			Monitor.Exit(_lock);
		}
	}

	public async Task UpdateAllAvatarFromGame()
	{
		_ = 1;
		try
		{
			Monitor.Enter(_lock);
			string text = await _network.GetAllAvatarsAsync();
			AvatarListDto avatarListDto = JsonConvert.DeserializeObject<AvatarListDto>(text);
			if (avatarListDto == null || avatarListDto.AvatarGuids == null)
			{
				throw new Exception("failed to deserialize: " + text);
			}
			await UpdateDataAndResources(avatarListDto.AvatarGuids);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error updating avatar data: " + ex.Message, "Error");
		}
		finally
		{
			FinishFetch();
			Monitor.Exit(_lock);
		}
	}

	private async Task UpdateDataAndResources(List<ulong> avatarGuidList)
	{
		await UpdateAllAvatarData(await FetchAvatarDataFromGame(avatarGuidList));
		await _dbContext.SaveChangesAsync(_cts.Token);
	}

	private async Task<List<AvatarDataDto>> FetchAvatarDataFromGame(List<ulong> avatarGuidList)
	{
		ResetFetchState();
		_fetchType = FetchType.AvatarMetadata;
		FetchTotal = (uint)avatarGuidList.Count;
		List<Exception> exceptions = new List<Exception>();
		List<Task<AvatarDataDto?>> tasks = new List<Task<AvatarDataDto>>();
		avatarGuidList.ForEach(delegate(ulong avatarGuid)
		{
			tasks.Add(FetchAvatarDataFromGame(avatarGuid).ContinueWith(delegate(Task<AvatarDataDto?> t)
			{
				if (t.IsFaulted)
				{
					exceptions.Add(t.Exception);
				}
				return t.Result;
			}));
		});
		if (exceptions.Count > 0)
		{
			throw exceptions[0];
		}
		return (from x in await Task.WhenAll(tasks)
			where x != null
			select (x)).ToList();
	}

	private async Task<AvatarDataDto?> FetchAvatarDataFromGame(ulong avatarGuid)
	{
		string text = await _network.GetAvatarInfoAsync(avatarGuid);
		AvatarDataDto? result = JsonConvert.DeserializeObject<AvatarDataDto>(text) ?? throw new Exception("failed to deserialize: " + text);
		FetchCurrent++;
		return result;
	}

	private async Task UpdateAllAvatarData(List<AvatarDataDto> avatarDataList)
	{
		ResetFetchState();
		_fetchType = FetchType.AvatarResource;
		FetchTotal = (uint)avatarDataList.Count;
		foreach (AvatarDataDto avatarData in avatarDataList)
		{
			await UpdateAvatarData(avatarData);
			FetchCurrent++;
		}
	}

	private async Task UpdateAvatarData(AvatarDataDto avatarData)
	{
		DbAvatar dbAvatar = await _dbContext.Avatars.FindAsync(avatarData.AvatarId);
		if (dbAvatar == null)
		{
			DbAvatar dbAvatar2 = new DbAvatar
			{
				AvatarId = avatarData.AvatarId
			};
			DbAvatar dbAvatar3 = dbAvatar2;
			dbAvatar3.Name = await GetAvatarName(avatarData.NameTextId);
			dbAvatar2.TextId = avatarData.NameTextId;
			dbAvatar2.IconName = avatarData.IconName;
			DbAvatar dbAvatar4 = dbAvatar2;
			dbAvatar4.IconBase64 = await GetAvatarImage(avatarData.IconName);
			dbAvatar = dbAvatar2;
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
		uint uid = _statusService.CurrentUid;
		if (await _dbContext.UserAvatars.FindAsync(avatarData.Guid) == null)
		{
			DbUserAvatar entity = new DbUserAvatar
			{
				Guid = avatarData.Guid,
				AvatarId = dbAvatar.AvatarId,
				UserUid = uid
			};
			_dbContext.UserAvatars.Add(entity);
		}
	}

	private async Task<string> GetAvatarName(uint nameTextId)
	{
		return await _network.GetTextAsync(nameTextId);
	}

	private async Task<string> GetAvatarImage(string imageName)
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
