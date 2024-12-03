// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.DbRelicAffix
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Config;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace RelicService.Data.Database
{
  [Table("relic_affix")]
  internal class DbRelicAffix
  {
    [Key]
    [Column("id")]
    [DatabaseGenerated]
    public int Id { get; set; }

    [Column("prop_type")]
    public FightPropType PropType { get; set; }

    [Column("prop_value")]
    public float PropValue { get; set; }

    [Column("relic_guid")]
    public ulong RelicGuid { get; set; }

    [ForeignKey("RelicGuid")]
    public DbRelicItem Relic { get; set; }
  }
}
