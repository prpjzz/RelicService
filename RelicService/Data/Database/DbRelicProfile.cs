﻿// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbRelicProfile
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("relic_profile")]
  internal class DbRelicProfile
  {
    [Key]
    [Column("id")]
    [DatabaseGenerated]
    public int Id { get; set; }

    [Column("profile_name")]
    public string ProfileName { get; set; } = string.Empty;

    [Column("avatar_guid")]
    public ulong AvatarGuid { get; set; }

    [Column("auto_equip")]
    public bool AutoEquip { get; set; }

    [Column("with_scene")]
    public List<uint> WithScene { get; set; } = new List<uint>();

    [ForeignKey("AvatarGuid")]
    public DbUserAvatar UserAvatar { get; set; }

    public ICollection<DbRelicProfileTeamContext> TeamContexts { get; set; } = (ICollection<DbRelicProfileTeamContext>) new List<DbRelicProfileTeamContext>();

    public ICollection<DbRelicItem> RelicItems { get; set; } = (ICollection<DbRelicItem>) new List<DbRelicItem>();
  }
}