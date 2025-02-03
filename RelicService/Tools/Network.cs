using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RelicService.Tools;

internal class Network
{
	private static readonly string SERVER_URL = "http://localhost:27115";

	private static readonly string GET_USER_UID = SERVER_URL + "/user/uid";

	private static readonly string GET_SCENE_ID = SERVER_URL + "/scene/id";

	private static readonly string SHOW_MESSAGE = SERVER_URL + "/show-message";

	private static readonly string GET_ALL_AVATARS = SERVER_URL + "/team-manager/avatars/all";

	private static readonly string GET_CURRENT_TEAM = SERVER_URL + "/team-manager/team/current";

	private static readonly string GET_AVATAR_INFO = SERVER_URL + "/avatar/{0}";

	private static readonly string GET_AVATAR_RELICS = SERVER_URL + "/avatar/{0}/relic/current";

	private static readonly string UPDATE_AVATAR_EQUIP = SERVER_URL + "/avatar/{0}/equip";

	private static readonly string GET_TEXT = SERVER_URL + "/textmap/{0}";

	private static readonly string GET_ITEM_IMAGE = SERVER_URL + "/sprite/image/{0}";

	private static readonly string OPEN_CRAFTING = SERVER_URL + "/ui/crafting";

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private readonly HttpClient _httpClient = new HttpClient();

	private readonly EventManager _eventManager;

	public Network(EventManager eventManager)
	{
		_eventManager = eventManager;
		_eventManager.OnShutdown += OnShutdown;
	}

	public async Task<string?> GetVersionInfo()
	{
		return await GetRequestAsync("https://rs.ex-m.net/rsclient/version");
	}

	public string GetSwaggerUrl()
	{
		return SERVER_URL + "/swagger/ui";
	}

	public async Task<string> GetUserUidAsync()
	{
		return await GetRequestAsync(GET_USER_UID);
	}

	public async Task<string> GetSceneIdAsync()
	{
		return await GetRequestAsync(GET_SCENE_ID);
	}

	public async Task ShowMessageAsync(string message)
	{
		await PostRequestAsync(SHOW_MESSAGE, message);
	}

	public async Task<string> GetAllAvatarsAsync()
	{
		return await GetRequestAsync(GET_ALL_AVATARS);
	}

	public async Task<string> GetCurrentTeamAsync()
	{
		return await GetRequestAsync(GET_CURRENT_TEAM);
	}

	public async Task<string> GetAvatarInfoAsync(ulong guid)
	{
		string url = string.Format(GET_AVATAR_INFO, guid);
		return await GetRequestAsync(url);
	}

	public async Task<string> GetAvatarRelicsAsync(ulong guid)
	{
		string url = string.Format(GET_AVATAR_RELICS, guid);
		return await GetRequestAsync(url);
	}

	public async Task UpdateAvatarEquipAsync(ulong avatarGuid, ulong relicGuid)
	{
		string url = string.Format(UPDATE_AVATAR_EQUIP, avatarGuid);
		await PutRequestAsync(url, new { relicGuid });
	}

	public async Task<string> GetTextAsync(uint textId)
	{
		string url = string.Format(GET_TEXT, textId);
		return await GetRequestAsync(url);
	}

	public async Task<string> GetItemImageAsync(string imageName)
	{
		string url = string.Format(GET_ITEM_IMAGE, imageName);
		return await GetRequestAsync(url);
	}

	public async Task OpenCrafting()
	{
		string oPEN_CRAFTING = OPEN_CRAFTING;
		await PostRequestAsync(oPEN_CRAFTING, string.Empty);
	}

	private async Task<string> GetRequestAsync(string url)
	{
		using (new StringContent(string.Empty, Encoding.UTF8, "text/plain"))
		{
			using HttpResponseMessage response = await _httpClient.GetAsync(url, _cts.Token);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
	}

	private async Task<string> PostRequestAsync(string url, string plain)
	{
		using StringContent content = new StringContent(plain, Encoding.UTF8, "text/plain");
		using HttpResponseMessage response = await _httpClient.PostAsync(url, content, _cts.Token);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync();
	}

	private async Task<string> PutRequestAsync(string url, object data)
	{
		string content = JsonConvert.SerializeObject(data);
		using StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
		using HttpResponseMessage response = await _httpClient.PutAsync(url, content2, _cts.Token);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync();
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
