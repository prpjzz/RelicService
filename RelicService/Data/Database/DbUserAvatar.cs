// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbUserAvatar
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("user_avatar")]
  internal class DbUserAvatar
  {
    [Key]
    [Column("guid")]
    public ulong Guid { get; set; }

    [Column("avatar_id")]
    public uint AvatarId { get; set; }

    [Column("user_uid")]
    public uint UserUid { get; set; }

    [ForeignKey("AvatarId")]
    public DbAvatar Avatar { get; set; }

    public ICollection<DbRelicProfile> RelicProfiles { get; set; } = (ICollection<DbRelicProfile>) new List<DbRelicProfile>();
  }
}
