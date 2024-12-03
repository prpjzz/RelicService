// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbAvatar
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("avatar")]
  internal class DbAvatar
  {
    [Key]
    [Column("id")]
    public uint AvatarId { get; set; }

    [Column("text_id")]
    public uint TextId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("icon_name")]
    public string IconName { get; set; } = string.Empty;

    [Column("icon_base64")]
    public string IconBase64 { get; set; } = string.Empty;
  }
}
