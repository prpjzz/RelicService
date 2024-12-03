// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Dto.RelicAffixDto
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Newtonsoft.Json;
using RelicService.Data.Config;

#nullable disable
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
