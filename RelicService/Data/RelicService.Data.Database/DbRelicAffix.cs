using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RelicService.Data.Config;

namespace RelicService.Data.Database;

[Table("relic_affix")]
internal class DbRelicAffix
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
