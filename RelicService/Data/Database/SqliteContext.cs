using Microsoft.EntityFrameworkCore;
using RelicService.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Database
{
    internal class SqliteContext : DbContext
    {
        public DbSet<DbAvatar> Avatars { get; set; }
        public DbSet<DbUserAvatar> UserAvatars { get; set; }
        public DbSet<DbRelic> Relics { get; set; }
        public DbSet<DbRelicItem> RelicItems { get; set; }
        public DbSet<DbRelicAffix> RelicAffixes { get; set; }
        public DbSet<DbRelicProfile> RelicProfiles { get; set; }
        public DbSet<DbRelicProfileTeamContext> RelicProfileTeamContext { get; set; }


        public SqliteContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // each user avatar should reference to its metadata
            // but the metadata should not reference to the user avatar
            modelBuilder.Entity<DbUserAvatar>()
                .HasOne(p => p.Avatar)
                .WithMany()
                .HasForeignKey(p => p.AvatarId)
                .IsRequired();

            // convert enum to string
            modelBuilder.Entity<DbRelicItem>()
                .Property(p => p.EquipType)
                .HasConversion(
                    v => v.ToString(),
                    v => (EquipType)Enum.Parse(typeof(EquipType), v)
                );

            // convert enum to string
            modelBuilder.Entity<DbRelicItem>()
                .Property(p => p.MainPropType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FightPropType)Enum.Parse(typeof(FightPropType), v)
                );

            // each relic should reference to its metadata
            // but the metadata should not reference to the relic
            modelBuilder.Entity<DbRelicItem>()
                .HasOne(p => p.Relic)
                .WithMany()
                .HasForeignKey(p => p.ItemId)
                .IsRequired();

            // convert enum to string
            modelBuilder.Entity<DbRelicAffix>()
                .Property(p => p.PropType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FightPropType)Enum.Parse(typeof(FightPropType), v)
                );

            // each affix should reference to its relic
            modelBuilder.Entity<DbRelicAffix>()
                .HasOne(p => p.Relic)
                .WithMany(p => p.Affixes)
                .HasForeignKey(p => p.RelicGuid)
                .IsRequired();

            // each relic should have multiple affixes
            modelBuilder.Entity<DbRelicItem>()
                .HasMany(p => p.Affixes)
                .WithOne(p => p.Relic)
                .HasForeignKey(p => p.RelicGuid)
                .IsRequired();

            // each profile have multiple relic item
            // but relic item should not reference to profile
            // becase relic item can be shared between profiles
            modelBuilder.Entity<DbRelicProfile>()
                .HasMany(p => p.RelicItems)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "relic_profile_item",
                    r => r.HasOne<DbRelicItem>().WithMany().HasForeignKey("relic_item_id"),
                    l => l.HasOne<DbRelicProfile>().WithMany().HasForeignKey("profile_id"),
                    r => r.HasKey("relic_item_id", "profile_id")
                );

            // each profile should reference to its avatar
            modelBuilder.Entity<DbRelicProfile>()
                .HasOne(p => p.UserAvatar)
                .WithMany()
                .HasForeignKey(p => p.AvatarGuid)
                .IsRequired();

            // convert list to csv
            modelBuilder.Entity<DbRelicProfile>()
                .Property(p => p.WithScene)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList()
                );

            // convert list to csv
            modelBuilder.Entity<DbRelicProfileTeamContext>()
                .Property(p => p.AvatarIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList()
                );

            // each profile can have multiple team context
            // but each team context should reference to only one profile
            modelBuilder.Entity<DbRelicProfile>()
                .HasMany(p => p.TeamContexts)
                .WithOne(p => p.Profile)
                .HasForeignKey(p => p.ProfileId)
                .IsRequired();

            // each avatar should have multiple profiles
            modelBuilder.Entity<DbUserAvatar>()
                .HasMany(p => p.RelicProfiles)
                .WithOne(p => p.UserAvatar)
                .HasForeignKey(p => p.AvatarGuid)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=RelicService.db");
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}
