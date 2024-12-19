using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RelicService.Data.Event;
using RelicService.Tools;

namespace RelicService.Service;

internal class StatusService
{
	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private bool _isServiceOnline;

	private uint _lastUid;

	private uint _lastSceneId;

	private List<ulong> _lastActiveTeam = new List<ulong>();

	private EventManager _eventManager;

	private Network _network;

	public bool IsServiceOnline
	{
		get
		{
			return _isServiceOnline;
		}
		private set
		{
			if (_isServiceOnline != value)
			{
				_isServiceOnline = value;
				_eventManager.FireEventAsync(EventId.EvtServiceStatusChanged, value);
			}
		}
	}

	public bool IsServiceOffline => !IsServiceOnline;

	public uint CurrentUid
	{
		get
		{
			return _lastUid;
		}
		private set
		{
			if (_lastUid != value)
			{
				_lastUid = value;
				_eventManager.FireEventAsync(EventId.EvtUidChanged, value);
			}
		}
	}

	public uint CurrentSceneId
	{
		get
		{
			return _lastSceneId;
		}
		private set
		{
			if (_lastSceneId != value)
			{
				_lastSceneId = value;
				_eventManager.FireEventAsync(EventId.EvtSceneIdChanged, value);
			}
		}
	}

	public StatusService(EventManager eventManager, Network network)
	{
		_eventManager = eventManager;
		_network = network;
		_eventManager.OnShutdown += OnShutdown;
		Task.Run((Func<Task?>)StatusPoll, _cts.Token);
	}

	private async Task StatusPoll()
	{
		while (!_cts.Token.IsCancellationRequested)
		{
			IsServiceOnline = await IsServiceAvailable();
			if (IsServiceOnline)
			{
				await UpdateStatus();
			}
			await Task.Delay(2000, _cts.Token);
		}
	}

	public async Task<bool> IsServiceAvailable()
	{
		try
		{
			await _network.GetUserUidAsync();
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	private async Task UpdateStatus()
	{
		_ = 1;
		try
		{
			CurrentSceneId = uint.Parse(await _network.GetSceneIdAsync());
			uint currentUid = uint.Parse(await _network.GetUserUidAsync());
			CurrentUid = currentUid;
		}
		catch (Exception)
		{
		}
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
