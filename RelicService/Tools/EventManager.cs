// Decompiled with JetBrains decompiler
// Type: RelicService.Tools.EventManager
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Event;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Tools
{
  internal class EventManager : IDisposable
  {
    private readonly BlockingCollection<(EventId eventId, object? data)> _eventQueue = new BlockingCollection<(EventId, object)>();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public event EventHandler<uint>? OnUidChanged;

    public event EventHandler<uint>? OnSceneIdChanged;

    public event EventHandler<bool>? OnServiceStatusChanged;

    public event EventHandler? OnShutdown;

    public event EventHandler<FetchProgressEvent>? OnFetchProgress;

    public event EventHandler? OnProfileRefresh;

    public event EventHandler<string>? OnProfileConflict;

    public EventManager()
    {
      Task.Factory.StartNew(new Action(this.ProcessEvents), this._cts.Token, (TaskCreationOptions) 2, TaskScheduler.Default);
    }

    public void FireEvent(EventId eventId, object? data = null)
    {
      switch (eventId)
      {
        case EventId.EvtUidChanged:
          this.ThrowOnNull(data);
          EventHandler<uint> onUidChanged = this.OnUidChanged;
          if (onUidChanged == null)
            break;
          onUidChanged((object) null, (uint) data);
          break;
        case EventId.EvtSceneIdChanged:
          this.ThrowOnNull(data);
          EventHandler<uint> onSceneIdChanged = this.OnSceneIdChanged;
          if (onSceneIdChanged == null)
            break;
          onSceneIdChanged((object) null, (uint) data);
          break;
        case EventId.EvtServiceStatusChanged:
          this.ThrowOnNull(data);
          EventHandler<bool> serviceStatusChanged = this.OnServiceStatusChanged;
          if (serviceStatusChanged == null)
            break;
          serviceStatusChanged((object) null, (bool) data);
          break;
        case EventId.EvtShutdown:
          EventHandler onShutdown = this.OnShutdown;
          if (onShutdown == null)
            break;
          onShutdown((object) null, EventArgs.Empty);
          break;
        case EventId.EvtFetchProgress:
          this.ThrowOnNull(data);
          EventHandler<FetchProgressEvent> onFetchProgress = this.OnFetchProgress;
          if (onFetchProgress == null)
            break;
          onFetchProgress((object) null, (FetchProgressEvent) data);
          break;
        case EventId.EvtProfileRefresh:
          EventHandler onProfileRefresh = this.OnProfileRefresh;
          if (onProfileRefresh == null)
            break;
          onProfileRefresh((object) null, EventArgs.Empty);
          break;
        case EventId.EvtProfileConflict:
          this.ThrowOnNull(data);
          EventHandler<string> onProfileConflict = this.OnProfileConflict;
          if (onProfileConflict == null)
            break;
          onProfileConflict((object) null, (string) data);
          break;
      }
    }

    public void FireEventAsync(EventId eventId, object? data = null)
    {
      this._eventQueue.Add((eventId, data));
    }

    private void ProcessEvents()
    {
      foreach ((EventId eventId, object data) in this._eventQueue.GetConsumingEnumerable(this._cts.Token))
      {
        try
        {
          this.FireEvent(eventId, data);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public void Dispose()
    {
      this._cts.Cancel();
      this._eventQueue.CompleteAdding();
    }

    private void ThrowOnNull(object? obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
    }
  }
}
