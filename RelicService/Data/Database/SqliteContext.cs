using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelicService.Data.Config;

namespace RelicService.Data.Database;

internal class SqliteContext : DbContext
{
	public DbSet<DbAvatar> Avatars { get; set; }

	public DbSet<DbUserAvatar> UserAvatars { get; set; }

	public DbSet<DbRelic> Relics { get; set; }

	public DbSet<DbRelicItem> RelicItems { get; set; }

	public DbSet<DbRelicAffix> RelicAffixes { get; set; }

	public DbSet<DbRelicProfile> RelicProfiles { get; set; }

	public DbSet<DbRelicProfileTeamContext> RelicProfileTeamContext { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DbUserAvatar>().HasOne((DbUserAvatar p) => p.Avatar).WithMany()
			.HasForeignKey((DbUserAvatar p) => p.AvatarId)
			.IsRequired();
		modelBuilder.Entity<DbRelicItem>().Property((DbRelicItem p) => p.EquipType).HasConversion((EquipType v) => v.ToString(), (string v) => (EquipType)Enum.Parse(typeof(EquipType), v));
		modelBuilder.Entity<DbRelicItem>().Property((DbRelicItem p) => p.MainPropType).HasConversion((FightPropType v) => v.ToString(), (string v) => (FightPropType)Enum.Parse(typeof(FightPropType), v));
		modelBuilder.Entity<DbRelicItem>().HasOne((DbRelicItem p) => p.Relic).WithMany()
			.HasForeignKey((DbRelicItem p) => p.ItemId)
			.IsRequired();
		modelBuilder.Entity<DbRelicAffix>().Property((DbRelicAffix p) => p.PropType).HasConversion((FightPropType v) => v.ToString(), (string v) => (FightPropType)Enum.Parse(typeof(FightPropType), v));
		modelBuilder.Entity<DbRelicAffix>().HasOne((DbRelicAffix p) => p.Relic).WithMany((DbRelicItem p) => p.Affixes)
			.HasForeignKey((DbRelicAffix p) => p.RelicGuid)
			.IsRequired();
		modelBuilder.Entity<DbRelicItem>().HasMany((DbRelicItem p) => p.Affixes).WithOne((DbRelicAffix p) => p.Relic)
			.HasForeignKey((DbRelicAffix p) => p.RelicGuid)
			.IsRequired();
		modelBuilder.Entity<DbRelicProfile>().HasMany((DbRelicProfile p) => p.RelicItems).WithMany()
			.UsingEntity("relic_profile_item", (EntityTypeBuilder<Dictionary<string, object>> r) => r.HasOne<DbRelicItem>().WithMany().HasForeignKey("relic_item_id"), (EntityTypeBuilder<Dictionary<string, object>> l) => l.HasOne<DbRelicProfile>().WithMany().HasForeignKey("profile_id"), delegate(EntityTypeBuilder<Dictionary<string, object>> r)
			{
				r.HasKey("relic_item_id", "profile_id");
			});
		modelBuilder.Entity<DbRelicProfile>().HasOne((DbRelicProfile p) => p.UserAvatar).WithMany()
			.HasForeignKey((DbRelicProfile p) => p.AvatarGuid)
			.IsRequired();
		modelBuilder.Entity<DbRelicProfile>().Property((DbRelicProfile p) => p.WithScene).HasConversion((List<uint> v) => string.Join(',', v), (string v) => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList());
		modelBuilder.Entity<DbRelicProfileTeamContext>().Property((DbRelicProfileTeamContext p) => p.AvatarIds).HasConversion((List<uint> v) => string.Join(',', v), (string v) => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList());
		modelBuilder.Entity<DbRelicProfile>().HasMany((DbRelicProfile p) => p.TeamContexts).WithOne((DbRelicProfileTeamContext p) => p.Profile)
			.HasForeignKey((DbRelicProfileTeamContext p) => p.ProfileId)
			.IsRequired();
		modelBuilder.Entity<DbUserAvatar>().HasMany((DbUserAvatar p) => p.RelicProfiles).WithOne((DbRelicProfile p) => p.UserAvatar)
			.HasForeignKey((DbRelicProfile p) => p.AvatarGuid)
			.IsRequired();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source=RelicService.db");
		optionsBuilder.EnableSensitiveDataLogging();
	}
}
