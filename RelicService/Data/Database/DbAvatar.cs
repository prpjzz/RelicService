using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RelicService.Data.Database;

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
