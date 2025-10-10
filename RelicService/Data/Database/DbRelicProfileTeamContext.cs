using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Database
{
    [Table("relic_profile_team")]
    internal class DbRelicProfileTeamContext
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("avatar_ids")]
        public List<uint> AvatarIds { get; set; } = new();

        [Column("profile_id")]
        public int ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public DbRelicProfile Profile { get; set; }
    }
}
