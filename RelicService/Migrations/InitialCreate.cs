// Decompiled with JetBrains decompiler
// Type: RelicService.Migrations.InitialCreate
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using RelicService.Data.Database;
using System;

#nullable enable
namespace RelicService.Migrations
{
  [DbContext(typeof (SqliteContext))]
  [Migration("20240924015329_InitialCreate")]
  public class InitialCreate : Migration
  {
    protected override void Up(
    #nullable disable
    MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable("avatar", table => new
      {
        id = table.Column<uint>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        text_id = table.Column<uint>("INTEGER"),
        name = table.Column<string>("TEXT"),
        icon_name = table.Column<string>("TEXT"),
        icon_base64 = table.Column<string>("TEXT")
      }, constraints: table => table.PrimaryKey("PK_avatar", x => x.id));
      migrationBuilder.CreateTable("relic", table => new
      {
        id = table.Column<uint>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        text_id = table.Column<uint>("INTEGER"),
        name = table.Column<string>("TEXT"),
        icon_name = table.Column<string>("TEXT"),
        icon_base64 = table.Column<string>("TEXT")
      }, constraints: table => table.PrimaryKey("PK_relic", x => x.id));
      migrationBuilder.CreateTable("user_avatar", table => new
      {
        guid = table.Column<ulong>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        avatar_id = table.Column<uint>("INTEGER"),
        user_uid = table.Column<uint>("INTEGER")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_user_avatar", x => x.guid);
        table.ForeignKey("FK_user_avatar_avatar_avatar_id", x => x.avatar_id, "avatar", "id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateTable("relic_item", table => new
      {
        guid = table.Column<ulong>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        level = table.Column<uint>("INTEGER"),
        main_prop_id = table.Column<uint>("INTEGER"),
        rank_level = table.Column<uint>("INTEGER"),
        equip_type = table.Column<string>("TEXT"),
        main_prop_type = table.Column<string>("TEXT"),
        main_prop_value = table.Column<float>("REAL"),
        item_id = table.Column<uint>("INTEGER")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_relic_item", x => x.guid);
        table.ForeignKey("FK_relic_item_relic_item_id", x => x.item_id, "relic", "id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateTable("relic_profile", table => new
      {
        id = table.Column<int>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        profile_name = table.Column<string>("TEXT"),
        avatar_guid = table.Column<ulong>("INTEGER"),
        auto_equip = table.Column<bool>("INTEGER"),
        with_scene = table.Column<string>("TEXT")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_relic_profile", x => x.id);
        table.ForeignKey("FK_relic_profile_user_avatar_avatar_guid", x => x.avatar_guid, "user_avatar", "guid", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateTable("relic_affix", table => new
      {
        id = table.Column<int>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        prop_type = table.Column<string>("TEXT"),
        prop_value = table.Column<float>("REAL"),
        relic_guid = table.Column<ulong>("INTEGER")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_relic_affix", x => x.id);
        table.ForeignKey("FK_relic_affix_relic_item_relic_guid", x => x.relic_guid, "relic_item", "guid", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateTable("relic_profile_item", table => new
      {
        relic_item_id = table.Column<ulong>("INTEGER"),
        profile_id = table.Column<int>("INTEGER")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_relic_profile_item", x => new
        {
          relic_item_id = x.relic_item_id,
          profile_id = x.profile_id
        });
        table.ForeignKey("FK_relic_profile_item_relic_item_relic_item_id", x => x.relic_item_id, "relic_item", "guid", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
        table.ForeignKey("FK_relic_profile_item_relic_profile_profile_id", x => x.profile_id, "relic_profile", "id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateTable("relic_profile_team", table => new
      {
        id = table.Column<int>("INTEGER").Annotation("Sqlite:Autoincrement", (object) true),
        avatar_ids = table.Column<string>("TEXT"),
        profile_id = table.Column<int>("INTEGER")
      }, constraints: table =>
      {
        table.PrimaryKey("PK_relic_profile_team", x => x.id);
        table.ForeignKey("FK_relic_profile_team_relic_profile_profile_id", x => x.profile_id, "relic_profile", "id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      });
      migrationBuilder.CreateIndex("IX_relic_affix_relic_guid", "relic_affix", "relic_guid");
      migrationBuilder.CreateIndex("IX_relic_item_item_id", "relic_item", "item_id");
      migrationBuilder.CreateIndex("IX_relic_profile_avatar_guid", "relic_profile", "avatar_guid");
      migrationBuilder.CreateIndex("IX_relic_profile_item_profile_id", "relic_profile_item", "profile_id");
      migrationBuilder.CreateIndex("IX_relic_profile_team_profile_id", "relic_profile_team", "profile_id");
      migrationBuilder.CreateIndex("IX_user_avatar_avatar_id", "user_avatar", "avatar_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("relic_affix");
      migrationBuilder.DropTable("relic_profile_item");
      migrationBuilder.DropTable("relic_profile_team");
      migrationBuilder.DropTable("relic_item");
      migrationBuilder.DropTable("relic_profile");
      migrationBuilder.DropTable("relic");
      migrationBuilder.DropTable("user_avatar");
      migrationBuilder.DropTable("avatar");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
      modelBuilder.HasAnnotation("ProductVersion", (object) "8.0.8");
      modelBuilder.Entity("RelicService.Data.Database.DbAvatar", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<uint>("AvatarId").ValueGeneratedOnAdd().HasColumnType<uint>("INTEGER").HasColumnName<uint>("id");
        b.Property<string>("IconBase64").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("icon_base64");
        b.Property<string>("IconName").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("icon_name");
        b.Property<string>("Name").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("name");
        b.Property<uint>("TextId").HasColumnType<uint>("INTEGER").HasColumnName<uint>("text_id");
        b.HasKey("AvatarId");
        b.ToTable("avatar");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelic", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<uint>("ItemId").ValueGeneratedOnAdd().HasColumnType<uint>("INTEGER").HasColumnName<uint>("id");
        b.Property<string>("IconBase64").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("icon_base64");
        b.Property<string>("IconName").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("icon_name");
        b.Property<string>("Name").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("name");
        b.Property<uint>("TextId").HasColumnType<uint>("INTEGER").HasColumnName<uint>("text_id");
        b.HasKey("ItemId");
        b.ToTable("relic");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicAffix", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("INTEGER").HasColumnName<int>("id");
        b.Property<string>("PropType").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("prop_type");
        b.Property<float>("PropValue").HasColumnType<float>("REAL").HasColumnName<float>("prop_value");
        b.Property<ulong>("RelicGuid").HasColumnType<ulong>("INTEGER").HasColumnName<ulong>("relic_guid");
        b.HasKey("Id");
        b.HasIndex("RelicGuid");
        b.ToTable("relic_affix");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<ulong>("Guid").ValueGeneratedOnAdd().HasColumnType<ulong>("INTEGER").HasColumnName<ulong>("guid");
        b.Property<string>("EquipType").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("equip_type");
        b.Property<uint>("ItemId").HasColumnType<uint>("INTEGER").HasColumnName<uint>("item_id");
        b.Property<uint>("Level").HasColumnType<uint>("INTEGER").HasColumnName<uint>("level");
        b.Property<uint>("MainPropId").HasColumnType<uint>("INTEGER").HasColumnName<uint>("main_prop_id");
        b.Property<string>("MainPropType").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("main_prop_type");
        b.Property<float>("MainPropValue").HasColumnType<float>("REAL").HasColumnName<float>("main_prop_value");
        b.Property<uint>("RankLevel").HasColumnType<uint>("INTEGER").HasColumnName<uint>("rank_level");
        b.HasKey("Guid");
        b.HasIndex("ItemId");
        b.ToTable("relic_item");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("INTEGER").HasColumnName<int>("id");
        b.Property<bool>("AutoEquip").HasColumnType<bool>("INTEGER").HasColumnName<bool>("auto_equip");
        b.Property<ulong>("AvatarGuid").HasColumnType<ulong>("INTEGER").HasColumnName<ulong>("avatar_guid");
        b.Property<string>("ProfileName").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("profile_name");
        b.Property<string>("WithScene").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("with_scene");
        b.HasKey("Id");
        b.HasIndex("AvatarGuid");
        b.ToTable("relic_profile");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicProfileTeamContext", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("INTEGER").HasColumnName<int>("id");
        b.Property<string>("AvatarIds").IsRequired(true).HasColumnType<string>("TEXT").HasColumnName<string>("avatar_ids");
        b.Property<int>("ProfileId").HasColumnType<int>("INTEGER").HasColumnName<int>("profile_id");
        b.HasKey("Id");
        b.HasIndex("ProfileId");
        b.ToTable("relic_profile_team");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<ulong>("Guid").ValueGeneratedOnAdd().HasColumnType<ulong>("INTEGER").HasColumnName<ulong>("guid");
        b.Property<uint>("AvatarId").HasColumnType<uint>("INTEGER").HasColumnName<uint>("avatar_id");
        b.Property<uint>("UserUid").HasColumnType<uint>("INTEGER").HasColumnName<uint>("user_uid");
        b.HasKey("Guid");
        b.HasIndex("AvatarId");
        b.ToTable("user_avatar");
      }));
      modelBuilder.Entity("relic_profile_item", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<ulong>("relic_item_id").HasColumnType<ulong>("INTEGER");
        b.Property<int>("profile_id").HasColumnType<int>("INTEGER");
        b.HasKey("relic_item_id", "profile_id");
        b.HasIndex("profile_id");
        b.ToTable("relic_profile_item");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicAffix", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbRelicItem", "Relic").WithMany("Affixes").HasForeignKey("RelicGuid").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Relic");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbRelic", "Relic").WithMany().HasForeignKey("ItemId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Relic");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbUserAvatar", "UserAvatar").WithMany("RelicProfiles").HasForeignKey("AvatarGuid").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("UserAvatar");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicProfileTeamContext", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbRelicProfile", "Profile").WithMany("TeamContexts").HasForeignKey("ProfileId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Profile");
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbAvatar", "Avatar").WithMany().HasForeignKey("AvatarId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Avatar");
      }));
      modelBuilder.Entity("relic_profile_item", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("RelicService.Data.Database.DbRelicProfile", (string) null).WithMany().HasForeignKey("profile_id").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.HasOne("RelicService.Data.Database.DbRelicItem", (string) null).WithMany().HasForeignKey("relic_item_id").OnDelete(DeleteBehavior.Cascade).IsRequired();
      }));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", (Action<EntityTypeBuilder>) (b => b.Navigation("Affixes")));
      modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", (Action<EntityTypeBuilder>) (b => b.Navigation("TeamContexts")));
      modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", (Action<EntityTypeBuilder>) (b => b.Navigation("RelicProfiles")));
    }
  }
}
