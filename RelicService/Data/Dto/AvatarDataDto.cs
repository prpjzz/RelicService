// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Dto.AvatarDataDto
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Newtonsoft.Json;

#nullable enable
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
