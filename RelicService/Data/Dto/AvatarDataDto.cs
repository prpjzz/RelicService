using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Dto
{
    internal class AvatarDataDto
    {
        [JsonProperty("configId")]
        public uint AvatarId { get; set; }

        [JsonProperty("nameTextId")]
        public uint NameTextId { get; set; }

        [JsonProperty("iconName")]
        public string IconName { get; set; } = string.Empty;

        [JsonProperty("guid")]
        public ulong Guid { get; set; }
    }
}
