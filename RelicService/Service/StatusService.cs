using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelicService.Data.Event;

namespace RelicService.Service
{
    internal class StatusService
    {
        private readonly CancellationTokenSource _cts = new();
        private bool _isServiceOnline = false;
        private uint _lastUid = 0;
        private uint _lastSceneId = 0;
        private List<ulong> _lastActiveTeam = new();

        EventManager _eventManager;
        Network _network;

        public bool IsServiceOnline { 
            get { return _isServiceOnline;  } 
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

        public uint CurrentUid { 
            get { return _lastUid; }
            private set
            {
                if (_lastUid != value)
                {
                    _lastUid = value;
                    _eventManager.FireEventAsync(EventId.EvtUidChanged, value);
                }
            }
        }
        public uint CurrentSceneId { 
            get { return _lastSceneId; }
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

            _ = Task.Run(StatusPoll, _cts.Token);
        }


        private async Task StatusPoll()
        {
            
            while (!_cts.Token.IsCancellationRequested)
            {

                IsServiceOnline = await IsServiceAvailable();
                if (IsServiceOnline)
                    await UpdateStatus();

                await Task.Delay(2000, _cts.Token);
            }

        }

        public async Task<bool> IsServiceAvailable()
        {
            try
            {
                _ = await _network.GetUserUidAsync();
            } 
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private async Task UpdateStatus()
        {
            try
            {
                var sceneId = await _network.GetSceneIdAsync();
                CurrentSceneId = uint.Parse(sceneId);

                var uidStr = await _network.GetUserUidAsync();
                var uid = uint.Parse(uidStr);
                CurrentUid = uid;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void OnShutdown(object? sender, EventArgs e)
        {
            _cts.Cancel();
        }

    }

}
