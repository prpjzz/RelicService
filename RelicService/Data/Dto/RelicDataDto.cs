// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Dto.RelicDataDto
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Newtonsoft.Json;
using RelicService.Data.Config;
using System.Collections.Generic;

#nullable enable
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
    public List<RelicAffixDto> AppendProp { get; set; } = new List<RelicAffixDto>();

    [JsonProperty("nameTextId")]
    public uint NameTextId { get; set; }

    [JsonProperty("iconPath")]
    public string IconName { get; set; } = string.Empty;
  }
}
