// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbRelicItem
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Config;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("relic_item")]
  internal class DbRelicItem
  {
    [Key]
    [Column("guid")]
    public ulong Guid { get; set; }

    [Column("level")]
    public uint Level { get; set; }

    [Column("main_prop_id")]
    public uint MainPropId { get; set; }

    [Column("rank_level")]
    public uint RankLevel { get; set; }

    [Column("equip_type")]
    public EquipType EquipType { get; set; }

    [Column("main_prop_type")]
    public FightPropType MainPropType { get; set; }

    [Column("main_prop_value")]
    public float MainPropValue { get; set; }

    [Column("item_id")]
    public uint ItemId { get; set; }

    [ForeignKey("ItemId")]
    public DbRelic Relic { get; set; }

    public ICollection<DbRelicAffix> Affixes { get; set; } = (ICollection<DbRelicAffix>) new List<DbRelicAffix>();
  }
}
