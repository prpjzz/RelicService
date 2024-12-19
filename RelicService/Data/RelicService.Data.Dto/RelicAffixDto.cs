using Newtonsoft.Json;
using RelicService.Data.Config;

namespace RelicService.Data.Dto;

internal class RelicAffixDto
{
	[JsonProperty("propType")]
	public FightPropType PropType { get; set; }

	[JsonProperty("propValue")]
	public float PropValue { get; set; }
}
