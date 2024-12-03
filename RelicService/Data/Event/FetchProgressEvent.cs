// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Event.FetchProgressEvent
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Config;

#nullable enable
namespace RelicService.Data.Event
{
  internal class FetchProgressEvent
  {
    public FetchType Type { get; set; }

    public uint Current { get; set; }

    public uint Total { get; set; }

    public string Name
    {
      get
      {
        string name;
        switch (this.Type)
        {
          case FetchType.AvatarMetadata:
            name = "角色元数据";
            break;
          case FetchType.AvatarResource:
            name = "角色资源";
            break;
          case FetchType.RelicMetadata:
            name = "圣遗物元数据";
            break;
          case FetchType.RelicResource:
            name = "圣遗物资源";
            break;
          default:
            name = "未知";
            break;
        }
        return name;
      }
    }

    public FetchProgressEvent(FetchType type, uint current, uint total)
    {
      this.Type = type;
      this.Current = current;
      this.Total = total;
    }
  }
}
