using RelicService.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Service
{
    internal class GameMessageService
    {
        private Network _network;
        private EventManager _eventManager;

        private ConcurrentQueue<string> _messageQueue = new();
        private readonly CancellationTokenSource _cts = new();

        public bool Enabled { get; set; } = true;

        public GameMessageService(
            Network network,
            EventManager eventManager
            )
        {
            _network = network;
            _eventManager = eventManager;

            _eventManager.OnShutdown += OnShutdown;
            
            _ = Task.Run(Worker, _cts.Token);
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
                if (!_messageQueue.TryDequeue(out var message))
                    continue;

                if (Enabled)
                    await _network.ShowMessageAsync(message);
            }

        }

        private void OnShutdown(object? sender, EventArgs e) => _cts.Cancel();

    }
}
