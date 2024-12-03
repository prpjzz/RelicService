// Decompiled with JetBrains decompiler
// Type: RelicService.Tools.Network
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Tools
{
  internal class Network
  {
    private static readonly string SERVER_URL = "http://localhost:27115";
    private static readonly string GET_USER_UID = Network.SERVER_URL + "/user/uid";
    private static readonly string GET_SCENE_ID = Network.SERVER_URL + "/scene/id";
    private static readonly string SHOW_MESSAGE = Network.SERVER_URL + "/show-message";
    private static readonly string GET_ALL_AVATARS = Network.SERVER_URL + "/team-manager/avatars/all";
    private static readonly string GET_CURRENT_TEAM = Network.SERVER_URL + "/team-manager/team/current";
    private static readonly string GET_AVATAR_INFO = Network.SERVER_URL + "/avatar/{0}";
    private static readonly string GET_AVATAR_RELICS = Network.SERVER_URL + "/avatar/{0}/relic/current";
    private static readonly string UPDATE_AVATAR_EQUIP = Network.SERVER_URL + "/avatar/{0}/equip";
    private static readonly string GET_TEXT = Network.SERVER_URL + "/textmap/{0}";
    private static readonly string GET_ITEM_IMAGE = Network.SERVER_URL + "/sprite/image/{0}";
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly EventManager _eventManager;

    public Network(EventManager eventManager)
    {
      this._eventManager = eventManager;
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
    }

    public async Task<string?> GetVersionInfo()
    {
      return await this.GetRequestAsync("https://rs.ex-m.net/rsclient/version");
    }

    public string GetSwaggerUrl() => Network.SERVER_URL + "/swagger/ui";

    public async Task<string> GetUserUidAsync() => await this.GetRequestAsync(Network.GET_USER_UID);

    public async Task<string> GetSceneIdAsync() => await this.GetRequestAsync(Network.GET_SCENE_ID);

    public async Task ShowMessageAsync(string message)
    {
      string str = await this.PostRequestAsync(Network.SHOW_MESSAGE, message);
    }

    public async Task<string> GetAllAvatarsAsync()
    {
      return await this.GetRequestAsync(Network.GET_ALL_AVATARS);
    }

    public async Task<string> GetCurrentTeamAsync()
    {
      return await this.GetRequestAsync(Network.GET_CURRENT_TEAM);
    }

    public async Task<string> GetAvatarInfoAsync(ulong guid)
    {
      return await this.GetRequestAsync(string.Format(Network.GET_AVATAR_INFO, (object) guid));
    }

    public async Task<string> GetAvatarRelicsAsync(ulong guid)
    {
      return await this.GetRequestAsync(string.Format(Network.GET_AVATAR_RELICS, (object) guid));
    }

    public async Task UpdateAvatarEquipAsync(ulong avatarGuid, ulong relicGuid)
    {
      string str = await this.PutRequestAsync(string.Format(Network.UPDATE_AVATAR_EQUIP, (object) avatarGuid), (object) new
      {
        relicGuid = relicGuid
      });
    }

    public async Task<string> GetTextAsync(uint textId)
    {
      return await this.GetRequestAsync(string.Format(Network.GET_TEXT, (object) textId));
    }

    public async Task<string> GetItemImageAsync(string imageName)
    {
      return await this.GetRequestAsync(string.Format(Network.GET_ITEM_IMAGE, (object) imageName));
    }

    private async Task<string> GetRequestAsync(string url)
    {
      HttpResponseMessage response;
      string requestAsync;
      using (new StringContent(string.Empty, Encoding.UTF8, "text/plain"))
      {
        response = await this._httpClient.GetAsync(url, this._cts.Token);
        try
        {
          response.EnsureSuccessStatusCode();
          requestAsync = await response.Content.ReadAsStringAsync();
        }
        finally
        {
          response?.Dispose();
        }
      }
      response = (HttpResponseMessage) null;
      return requestAsync;
    }

    private async Task<string> PostRequestAsync(string url, string plain)
    {
      HttpResponseMessage response;
      string str;
      using (StringContent content = new StringContent(plain, Encoding.UTF8, "text/plain"))
      {
        response = await this._httpClient.PostAsync(url, (HttpContent) content, this._cts.Token);
        try
        {
          response.EnsureSuccessStatusCode();
          str = await response.Content.ReadAsStringAsync();
        }
        finally
        {
          response?.Dispose();
        }
      }
      response = (HttpResponseMessage) null;
      return str;
    }

    private async Task<string> PutRequestAsync(string url, object data)
    {
      HttpResponseMessage response;
      string str;
      using (StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
      {
        response = await this._httpClient.PutAsync(url, (HttpContent) content, this._cts.Token);
        try
        {
          response.EnsureSuccessStatusCode();
          str = await response.Content.ReadAsStringAsync();
        }
        finally
        {
          response?.Dispose();
        }
      }
      response = (HttpResponseMessage) null;
      return str;
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
