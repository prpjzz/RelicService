using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RelicService.Data.Database;

[Table("relic_profile")]
internal class DbRelicProfile
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

	public ICollection<DbRelicProfileTeamContext> TeamContexts { get; set; } = new List<DbRelicProfileTeamContext>();

	public ICollection<DbRelicItem> RelicItems { get; set; } = new List<DbRelicItem>();
}
