using Newtonsoft.Json;
using RelicService.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Dto
{
    internal class RelicAffixDto
    {
        [JsonProperty("propType")]
        public FightPropType PropType { get; set; }

        [JsonProperty("propValue")]
        public float PropValue { get; set; }
    }
}
