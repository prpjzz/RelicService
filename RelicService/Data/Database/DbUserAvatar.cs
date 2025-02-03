using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RelicService.Data.Database;

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

	public ICollection<DbRelicProfile> RelicProfiles { get; set; } = new List<DbRelicProfile>();
}
