// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbRelicProfileTeamContext
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("relic_profile_team")]
  internal class DbRelicProfileTeamContext
  {
    [Key]
    [Column("id")]
    [DatabaseGenerated]
    public int Id { get; set; }

    [Column("avatar_ids")]
    public List<uint> AvatarIds { get; set; } = new List<uint>();

    [Column("profile_id")]
    public int ProfileId { get; set; }

    [ForeignKey("ProfileId")]
    public DbRelicProfile Profile { get; set; }
  }
}
