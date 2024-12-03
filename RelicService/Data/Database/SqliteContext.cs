// Decompiled with JetBrains decompiler
// Type: RelicService.Data.Database.SqliteContext
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelicService.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<DbUserAvatar>().HasOne<DbAvatar>((Expression<Func<DbUserAvatar, DbAvatar>>) (p => p.Avatar)).WithMany((string) null).HasForeignKey((Expression<Func<DbUserAvatar, object>>) (p => (object) p.AvatarId)).IsRequired(true);
      ParameterExpression parameterExpression1;
      // ISSUE: method reference
      modelBuilder.Entity<DbRelicItem>().Property<EquipType>((Expression<Func<DbRelicItem, EquipType>>) (p => p.EquipType)).HasConversion<string>(Expression.Lambda<Func<EquipType, string>>((Expression) Expression.Call(v, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), new ParameterExpression[1]
      {
        parameterExpression1
      }), (Expression<Func<string, EquipType>>) (v => (EquipType) Enum.Parse(typeof (EquipType), v)));
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      modelBuilder.Entity<DbRelicItem>().Property<FightPropType>((Expression<Func<DbRelicItem, FightPropType>>) (p => p.MainPropType)).HasConversion<string>(Expression.Lambda<Func<FightPropType, string>>((Expression) Expression.Call(v, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), new ParameterExpression[1]
      {
        parameterExpression2
      }), (Expression<Func<string, FightPropType>>) (v => (FightPropType) Enum.Parse(typeof (FightPropType), v)));
      modelBuilder.Entity<DbRelicItem>().HasOne<DbRelic>((Expression<Func<DbRelicItem, DbRelic>>) (p => p.Relic)).WithMany((string) null).HasForeignKey((Expression<Func<DbRelicItem, object>>) (p => (object) p.ItemId)).IsRequired(true);
      ParameterExpression parameterExpression3;
      // ISSUE: method reference
      modelBuilder.Entity<DbRelicAffix>().Property<FightPropType>((Expression<Func<DbRelicAffix, FightPropType>>) (p => p.PropType)).HasConversion<string>(Expression.Lambda<Func<FightPropType, string>>((Expression) Expression.Call(v, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>()), new ParameterExpression[1]
      {
        parameterExpression3
      }), (Expression<Func<string, FightPropType>>) (v => (FightPropType) Enum.Parse(typeof (FightPropType), v)));
      modelBuilder.Entity<DbRelicAffix>().HasOne<DbRelicItem>((Expression<Func<DbRelicAffix, DbRelicItem>>) (p => p.Relic)).WithMany((Expression<Func<DbRelicItem, IEnumerable<DbRelicAffix>>>) (p => p.Affixes)).HasForeignKey((Expression<Func<DbRelicAffix, object>>) (p => (object) p.RelicGuid)).IsRequired(true);
      modelBuilder.Entity<DbRelicItem>().HasMany<DbRelicAffix>((Expression<Func<DbRelicItem, IEnumerable<DbRelicAffix>>>) (p => p.Affixes)).WithOne((Expression<Func<DbRelicAffix, DbRelicItem>>) (p => p.Relic)).HasForeignKey((Expression<Func<DbRelicAffix, object>>) (p => (object) p.RelicGuid)).IsRequired(true);
      modelBuilder.Entity<DbRelicProfile>().HasMany<DbRelicItem>((Expression<Func<DbRelicProfile, IEnumerable<DbRelicItem>>>) (p => p.RelicItems)).WithMany((string) null).UsingEntity<Dictionary<string, object>>("relic_profile_item", (Func<EntityTypeBuilder<Dictionary<string, object>>, ReferenceCollectionBuilder<DbRelicItem, Dictionary<string, object>>>) (r => r.HasOne<DbRelicItem>().WithMany((string) null).HasForeignKey(new string[1]
      {
        "relic_item_id"
      })), (Func<EntityTypeBuilder<Dictionary<string, object>>, ReferenceCollectionBuilder<DbRelicProfile, Dictionary<string, object>>>) (l => l.HasOne<DbRelicProfile>().WithMany((string) null).HasForeignKey(new string[1]
      {
        "profile_id"
      })), (Action<EntityTypeBuilder<Dictionary<string, object>>>) (r => r.HasKey(new string[2]
      {
        "relic_item_id",
        "profile_id"
      })));
      modelBuilder.Entity<DbRelicProfile>().HasOne<DbUserAvatar>((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).WithMany((string) null).HasForeignKey((Expression<Func<DbRelicProfile, object>>) (p => (object) p.AvatarGuid)).IsRequired(true);
      // ISSUE: method reference
      modelBuilder.Entity<DbRelicProfile>().Property<List<uint>>((Expression<Func<DbRelicProfile, List<uint>>>) (p => p.WithScene)).HasConversion<string>((Expression<Func<List<uint>, string>>) (v => string.Join<uint>(',', v)), (Expression<Func<string, List<uint>>>) (v => Enumerable.ToList<uint>(Enumerable.Select<string, uint>(v.Split(',', (StringSplitOptions) 1), (Func<string, uint>) ((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (uint.Parse))).CreateDelegate(typeof (Func<string, uint>), default (object))))));
      // ISSUE: method reference
      modelBuilder.Entity<DbRelicProfileTeamContext>().Property<List<uint>>((Expression<Func<DbRelicProfileTeamContext, List<uint>>>) (p => p.AvatarIds)).HasConversion<string>((Expression<Func<List<uint>, string>>) (v => string.Join<uint>(',', v)), (Expression<Func<string, List<uint>>>) (v => Enumerable.ToList<uint>(Enumerable.Select<string, uint>(v.Split(',', (StringSplitOptions) 1), (Func<string, uint>) ((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (uint.Parse))).CreateDelegate(typeof (Func<string, uint>), default (object))))));
      modelBuilder.Entity<DbRelicProfile>().HasMany<DbRelicProfileTeamContext>((Expression<Func<DbRelicProfile, IEnumerable<DbRelicProfileTeamContext>>>) (p => p.TeamContexts)).WithOne((Expression<Func<DbRelicProfileTeamContext, DbRelicProfile>>) (p => p.Profile)).HasForeignKey((Expression<Func<DbRelicProfileTeamContext, object>>) (p => (object) p.ProfileId)).IsRequired(true);
      modelBuilder.Entity<DbUserAvatar>().HasMany<DbRelicProfile>((Expression<Func<DbUserAvatar, IEnumerable<DbRelicProfile>>>) (p => p.RelicProfiles)).WithOne((Expression<Func<DbRelicProfile, DbUserAvatar>>) (p => p.UserAvatar)).HasForeignKey((Expression<Func<DbRelicProfile, object>>) (p => (object) p.AvatarGuid)).IsRequired(true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=RelicService.db");
      optionsBuilder.EnableSensitiveDataLogging();
    }
  }
}
