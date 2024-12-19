using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RelicService.Tools;

namespace RelicService.Service;

internal class GameMessageService
{
	private Network _network;

	private EventManager _eventManager;

	private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	public bool Enabled { get; set; } = true;

	public GameMessageService(Network network, EventManager eventManager)
	{
		_network = network;
		_eventManager = eventManager;
		_eventManager.OnShutdown += OnShutdown;
		Task.Run((Func<Task?>)Worker, _cts.Token);
	}

	public void EnqueueMessage(string message)
	{
		_messageQueue.Enqueue(message);
	}

	private async Task Worker()
	{
		while (!_cts.IsCancellationRequested)
		{
			await Task.Delay(200);
			if (_messageQueue.TryDequeue(out string result) && Enabled)
			{
				await _network.ShowMessageAsync(result);
			}
		}
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
