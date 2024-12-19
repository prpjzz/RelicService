using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RelicService.Data.Config;

namespace RelicService.Data.Database;

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

	public ICollection<DbRelicAffix> Affixes { get; set; } = new List<DbRelicAffix>();
}
