// Decompiled with JetBrains decompiler
// Type: RelicService.Service.StatusService
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Event;
using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Service
{
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
      get => this._isServiceOnline;
      private set
      {
        if (this._isServiceOnline == value)
          return;
        this._isServiceOnline = value;
        this._eventManager.FireEventAsync(EventId.EvtServiceStatusChanged, (object) value);
      }
    }

    public bool IsServiceOffline => !this.IsServiceOnline;

    public uint CurrentUid
    {
      get => this._lastUid;
      private set
      {
        if ((int) this._lastUid == (int) value)
          return;
        this._lastUid = value;
        this._eventManager.FireEventAsync(EventId.EvtUidChanged, (object) value);
      }
    }

    public uint CurrentSceneId
    {
      get => this._lastSceneId;
      private set
      {
        if ((int) this._lastSceneId == (int) value)
          return;
        this._lastSceneId = value;
        this._eventManager.FireEventAsync(EventId.EvtSceneIdChanged, (object) value);
      }
    }

    public StatusService(EventManager eventManager, Network network)
    {
      this._eventManager = eventManager;
      this._network = network;
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
      Task.Run(new Func<Task>(this.StatusPoll), this._cts.Token);
    }

    private async Task StatusPoll()
    {
      while (!this._cts.Token.IsCancellationRequested)
      {
        this.IsServiceOnline = await this.IsServiceAvailable();
        if (this.IsServiceOnline)
          await this.UpdateStatus();
        await Task.Delay(2000, this._cts.Token);
      }
    }

    public async Task<bool> IsServiceAvailable()
    {
      try
      {
        string userUidAsync = await this._network.GetUserUidAsync();
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private async Task UpdateStatus()
    {
      try
      {
        this.CurrentSceneId = uint.Parse(await this._network.GetSceneIdAsync());
        this.CurrentUid = uint.Parse(await this._network.GetUserUidAsync());
      }
      catch (Exception ex)
      {
      }
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
