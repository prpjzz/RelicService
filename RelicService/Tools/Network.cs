using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Tools
{
    internal class Network
    {
        static readonly string SERVER_URL = "http://localhost:27115";
        static readonly string GET_USER_UID = $"{SERVER_URL}/user/uid";
        static readonly string GET_SCENE_ID = $"{SERVER_URL}/scene/id";
        static readonly string SHOW_MESSAGE = $"{SERVER_URL}/show-message";
        static readonly string GET_ALL_AVATARS = $"{SERVER_URL}/team-manager/avatars/all";
        static readonly string GET_CURRENT_TEAM = $"{SERVER_URL}/team-manager/team/current";
        static readonly string GET_AVATAR_INFO = $"{SERVER_URL}/avatar/{{0}}";
        static readonly string GET_AVATAR_RELICS = $"{SERVER_URL}/avatar/{{0}}/relic/current";
        static readonly string UPDATE_AVATAR_EQUIP = $"{SERVER_URL}/avatar/{{0}}/equip";
        static readonly string GET_TEXT = $"{SERVER_URL}/textmap/{{0}}";
        static readonly string GET_ITEM_IMAGE = $"{SERVER_URL}/sprite/image/{{0}}";
        static readonly string OPEN_CRAFTING = $"{SERVER_URL}/ui/crafting";

        private readonly CancellationTokenSource _cts = new();
        private readonly HttpClient _httpClient = new();
        private readonly EventManager _eventManager;

        public Network(EventManager eventManager)
        {
            _eventManager = eventManager;
            _eventManager.OnShutdown += OnShutdown;
        }

        public async Task<string?> GetVersionInfo()
        {
            var result = await GetRequestAsync("https://rs.ex-m.net/rsclient/version");
            return result;
        }

        public string GetSwaggerUrl()
        {
            return $"{SERVER_URL}/swagger/ui";
        }

        public async Task<string> GetUserUidAsync()
        {
            var result = await GetRequestAsync(GET_USER_UID);
            return result;
        }

        public async Task<string> GetSceneIdAsync()
        {
            var result = await GetRequestAsync(GET_SCENE_ID);
            return result;
        }

        public async Task ShowMessageAsync(string message)
        {
            _ = await PostRequestAsync(SHOW_MESSAGE, message);
        }

        public async Task<string> GetAllAvatarsAsync()
        {
            var result = await GetRequestAsync(GET_ALL_AVATARS);
            return result;
        }

        public async Task<string> GetCurrentTeamAsync()
        {
            var result = await GetRequestAsync(GET_CURRENT_TEAM);
            return result;
        }

        public async Task<string> GetAvatarInfoAsync(ulong guid)
        {
            var url = string.Format(GET_AVATAR_INFO, guid);
            var result = await GetRequestAsync(url);
            return result;
        }

        public async Task<string> GetAvatarRelicsAsync(ulong guid)
        {
            var url = string.Format(GET_AVATAR_RELICS, guid);
            var result = await GetRequestAsync(url);
            return result;
        }

        public async Task UpdateAvatarEquipAsync(ulong avatarGuid, ulong relicGuid)
        {
            var url = string.Format(UPDATE_AVATAR_EQUIP, avatarGuid);
            _ = await PutRequestAsync(url, new { relicGuid = relicGuid });
        }

        public async Task<string> GetTextAsync(uint textId)
        {
            var url = string.Format(GET_TEXT, textId);
            var result = await GetRequestAsync(url);
            return result;
        }

        public async Task<string> GetItemImageAsync(string imageName)
        {
            var url = string.Format(GET_ITEM_IMAGE, imageName);
            var result = await GetRequestAsync(url);
            return result;
        }

        public async Task OpenCrafting()
        {
            var url = OPEN_CRAFTING;
            _ = await PostRequestAsync(url, string.Empty);
        }

        private async Task<string> GetRequestAsync(string url)
        {
            using var content = new StringContent(string.Empty, Encoding.UTF8, "text/plain");
            using var response = await _httpClient.GetAsync(url, _cts.Token);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private async Task<string> PostRequestAsync(string url, string plain)
        {
            using var content = new StringContent(plain, Encoding.UTF8, "text/plain");
            using var response = await _httpClient.PostAsync(url, content, _cts.Token);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private async Task<string> PutRequestAsync(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PutAsync(url, content, _cts.Token);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private void OnShutdown(object? sender, EventArgs e) => _cts.Cancel();

    }
}
