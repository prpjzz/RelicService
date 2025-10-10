using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RelicService.Data.Event;

namespace RelicService.Tools
{
    internal class EventManager : IDisposable
    {
        private readonly BlockingCollection<(EventId eventId, object? data)> _eventQueue = new();
        private readonly CancellationTokenSource _cts = new();

        public event EventHandler<uint>? OnUidChanged;
        public event EventHandler<uint>? OnSceneIdChanged;
        public event EventHandler<bool>? OnServiceStatusChanged;
        public event EventHandler? OnShutdown;
        public event EventHandler<FetchProgressEvent>? OnFetchProgress;
        public event EventHandler? OnProfileRefresh;
        public event EventHandler<string>? OnProfileConflict;

        public EventManager()
        {
            
            Task.Factory.StartNew(ProcessEvents, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void FireEvent(EventId eventId, object? data = null)
        {
            switch (eventId)
            {
                case EventId.EvtUidChanged:
                    ThrowOnNull(data);
                    OnUidChanged?.Invoke(null, (uint)data);
                    break;
                case EventId.EvtSceneIdChanged:
                    ThrowOnNull(data);
                    OnSceneIdChanged?.Invoke(null, (uint)data);
                    break;
                case EventId.EvtServiceStatusChanged:
                    ThrowOnNull(data);
                    OnServiceStatusChanged?.Invoke(null, (bool)data);
                    break;
                case EventId.EvtShutdown:
                    OnShutdown?.Invoke(null, EventArgs.Empty);
                    break;
                case EventId.EvtFetchProgress:
                    ThrowOnNull(data);
                    OnFetchProgress?.Invoke(null, (FetchProgressEvent)data);
                    break;
                case EventId.EvtProfileRefresh:
                    OnProfileRefresh?.Invoke(null, EventArgs.Empty);
                    break;
                case EventId.EvtProfileConflict:
                    ThrowOnNull(data);
                    OnProfileConflict?.Invoke(null, (string)data);
                    break;
            }

        }

        public void FireEventAsync(EventId eventId, object? data = null)
        {
            _eventQueue.Add((eventId, data));
        }

        private void ProcessEvents()
        {
            foreach (var (eventId, data) in _eventQueue.GetConsumingEnumerable(_cts.Token))
            {
                try
                {
                    FireEvent(eventId, data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"EventManager.ProcessEvents: {ex}");
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _eventQueue.CompleteAdding();
        }

        private void ThrowOnNull(object? obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }

    }

}
