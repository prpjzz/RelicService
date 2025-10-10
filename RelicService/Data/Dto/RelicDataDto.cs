using Newtonsoft.Json;
using RelicService.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Dto
{
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
        public List<RelicAffixDto> AppendProp { get; set; } = new();

        [JsonProperty("nameTextId")]
        public uint NameTextId { get; set; }

        [JsonProperty("iconPath")]
        public string IconName { get; set; } = string.Empty;
    }
}
