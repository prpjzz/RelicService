using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Dto
{
    internal class AvatarListDto
    {
        [JsonProperty("avatars")]
        public List<ulong> AvatarGuids { get; set; } = new();
    }
}
