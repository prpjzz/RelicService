using System.Collections.Generic;
using Newtonsoft.Json;
using RelicService.Data.Config;

namespace RelicService.Data.Dto;

internal class RelicDataDto
{
	[JsonProperty("itemId")]
	public uint ItemId { get; set; }

	[JsonProperty("guid")]
	public ulong Guid { get; set; }

	[JsonProperty("level")]
	public uint Level { get; set; }

	[JsonProperty("mainPropId")]
	public uint MainPropId { get; set; }

	[JsonProperty("rankLevel")]
	public uint RankLevel { get; set; }

	[JsonProperty("equipType")]
	public EquipType EquipType { get; set; }

	[JsonProperty("mainPropType")]
	public FightPropType MainPropType { get; set; }

	[JsonProperty("mainPropValue")]
	public float MainPropValue { get; set; }

	[JsonProperty("appendProp")]
	public List<RelicAffixDto> AppendProp { get; set; } = new List<RelicAffixDto>();

	[JsonProperty("nameTextId")]
	public uint NameTextId { get; set; }

	[JsonProperty("iconPath")]
	public string IconName { get; set; } = string.Empty;
}
