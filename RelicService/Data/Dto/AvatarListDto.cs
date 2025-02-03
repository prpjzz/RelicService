using System.Collections.Generic;
using Newtonsoft.Json;

namespace RelicService.Data.Dto;

internal class AvatarListDto
{
	[JsonProperty("avatars")]
	public List<ulong> AvatarGuids { get; set; } = new List<ulong>();
}
