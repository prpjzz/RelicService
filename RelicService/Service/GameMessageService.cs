// Decompiled with JetBrains decompiler
// Type: RelicService.Service.GameMessageService
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Tools;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Service
{
  internal class GameMessageService
  {
    private Network _network;
    private EventManager _eventManager;
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public bool Enabled { get; set; } = true;

    public GameMessageService(Network network, EventManager eventManager)
    {
      this._network = network;
      this._eventManager = eventManager;
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
      Task.Run(new Func<Task>(this.Worker), this._cts.Token);
    }

    public void EnqueueMessage(string message) => this._messageQueue.Enqueue(message);

    private async Task Worker()
    {
      while (!this._cts.IsCancellationRequested)
      {
        await Task.Delay(200);
        string message;
        if (this._messageQueue.TryDequeue(ref message) && this.Enabled)
          await this._network.ShowMessageAsync(message);
      }
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
