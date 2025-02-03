using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using RelicService.Data.Database;

namespace RelicService.Migrations;

[DbContext(typeof(SqliteContext))]
[Migration("20240924015329_InitialCreate")]
public class InitialCreate : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("avatar", (ColumnsBuilder table) => new
		{
			id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			text_id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			name = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			icon_name = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			icon_base64 = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_avatar", x => x.id);
		});
		migrationBuilder.CreateTable("relic", (ColumnsBuilder table) => new
		{
			id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			text_id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			name = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			icon_name = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			icon_base64 = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic", x => x.id);
		});
		migrationBuilder.CreateTable("user_avatar", (ColumnsBuilder table) => new
		{
			guid = table.Column<ulong>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			avatar_id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			user_uid = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_user_avatar", x => x.guid);
			table.ForeignKey("FK_user_avatar_avatar_avatar_id", x => x.avatar_id, "avatar", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("relic_item", (ColumnsBuilder table) => new
		{
			guid = table.Column<ulong>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			level = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			main_prop_id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			rank_level = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			equip_type = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			main_prop_type = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			main_prop_value = table.Column<float>("REAL", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			item_id = table.Column<uint>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic_item", x => x.guid);
			table.ForeignKey("FK_relic_item_relic_item_id", x => x.item_id, "relic", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("relic_profile", (ColumnsBuilder table) => new
		{
			id = table.Column<int>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			profile_name = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			avatar_guid = table.Column<ulong>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			auto_equip = table.Column<bool>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			with_scene = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic_profile", x => x.id);
			table.ForeignKey("FK_relic_profile_user_avatar_avatar_guid", x => x.avatar_guid, "user_avatar", "guid", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("relic_affix", (ColumnsBuilder table) => new
		{
			id = table.Column<int>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			prop_type = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			prop_value = table.Column<float>("REAL", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			relic_guid = table.Column<ulong>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic_affix", x => x.id);
			table.ForeignKey("FK_relic_affix_relic_item_relic_guid", x => x.relic_guid, "relic_item", "guid", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("relic_profile_item", (ColumnsBuilder table) => new
		{
			relic_item_id = table.Column<ulong>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			profile_id = table.Column<int>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic_profile_item", x => new { x.relic_item_id, x.profile_id });
			table.ForeignKey("FK_relic_profile_item_relic_item_relic_item_id", x => x.relic_item_id, "relic_item", "guid", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_relic_profile_item_relic_profile_profile_id", x => x.profile_id, "relic_profile", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("relic_profile_team", (ColumnsBuilder table) => new
		{
			id = table.Column<int>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null).Annotation("Sqlite:Autoincrement", true),
			avatar_ids = table.Column<string>("TEXT", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null),
			profile_id = table.Column<int>("INTEGER", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, null, null, null)
		}, null, table =>
		{
			table.PrimaryKey("PK_relic_profile_team", x => x.id);
			table.ForeignKey("FK_relic_profile_team_relic_profile_profile_id", x => x.profile_id, "relic_profile", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
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
		modelBuilder.HasAnnotation("ProductVersion", "8.0.8");
		modelBuilder.Entity("RelicService.Data.Database.DbAvatar", delegate(EntityTypeBuilder b)
		{
			b.Property<uint>("AvatarId").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("id");
			b.Property<string>("IconBase64").IsRequired().HasColumnType("TEXT")
				.HasColumnName("icon_base64");
			b.Property<string>("IconName").IsRequired().HasColumnType("TEXT")
				.HasColumnName("icon_name");
			b.Property<string>("Name").IsRequired().HasColumnType("TEXT")
				.HasColumnName("name");
			b.Property<uint>("TextId").HasColumnType("INTEGER").HasColumnName("text_id");
			b.HasKey("AvatarId");
			b.ToTable("avatar");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelic", delegate(EntityTypeBuilder b)
		{
			b.Property<uint>("ItemId").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("id");
			b.Property<string>("IconBase64").IsRequired().HasColumnType("TEXT")
				.HasColumnName("icon_base64");
			b.Property<string>("IconName").IsRequired().HasColumnType("TEXT")
				.HasColumnName("icon_name");
			b.Property<string>("Name").IsRequired().HasColumnType("TEXT")
				.HasColumnName("name");
			b.Property<uint>("TextId").HasColumnType("INTEGER").HasColumnName("text_id");
			b.HasKey("ItemId");
			b.ToTable("relic");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicAffix", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("id");
			b.Property<string>("PropType").IsRequired().HasColumnType("TEXT")
				.HasColumnName("prop_type");
			b.Property<float>("PropValue").HasColumnType("REAL").HasColumnName("prop_value");
			b.Property<ulong>("RelicGuid").HasColumnType("INTEGER").HasColumnName("relic_guid");
			b.HasKey("Id");
			b.HasIndex("RelicGuid");
			b.ToTable("relic_affix");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", delegate(EntityTypeBuilder b)
		{
			b.Property<ulong>("Guid").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("guid");
			b.Property<string>("EquipType").IsRequired().HasColumnType("TEXT")
				.HasColumnName("equip_type");
			b.Property<uint>("ItemId").HasColumnType("INTEGER").HasColumnName("item_id");
			b.Property<uint>("Level").HasColumnType("INTEGER").HasColumnName("level");
			b.Property<uint>("MainPropId").HasColumnType("INTEGER").HasColumnName("main_prop_id");
			b.Property<string>("MainPropType").IsRequired().HasColumnType("TEXT")
				.HasColumnName("main_prop_type");
			b.Property<float>("MainPropValue").HasColumnType("REAL").HasColumnName("main_prop_value");
			b.Property<uint>("RankLevel").HasColumnType("INTEGER").HasColumnName("rank_level");
			b.HasKey("Guid");
			b.HasIndex("ItemId");
			b.ToTable("relic_item");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("id");
			b.Property<bool>("AutoEquip").HasColumnType("INTEGER").HasColumnName("auto_equip");
			b.Property<ulong>("AvatarGuid").HasColumnType("INTEGER").HasColumnName("avatar_guid");
			b.Property<string>("ProfileName").IsRequired().HasColumnType("TEXT")
				.HasColumnName("profile_name");
			b.Property<string>("WithScene").IsRequired().HasColumnType("TEXT")
				.HasColumnName("with_scene");
			b.HasKey("Id");
			b.HasIndex("AvatarGuid");
			b.ToTable("relic_profile");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicProfileTeamContext", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("id");
			b.Property<string>("AvatarIds").IsRequired().HasColumnType("TEXT")
				.HasColumnName("avatar_ids");
			b.Property<int>("ProfileId").HasColumnType("INTEGER").HasColumnName("profile_id");
			b.HasKey("Id");
			b.HasIndex("ProfileId");
			b.ToTable("relic_profile_team");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", delegate(EntityTypeBuilder b)
		{
			b.Property<ulong>("Guid").ValueGeneratedOnAdd().HasColumnType("INTEGER")
				.HasColumnName("guid");
			b.Property<uint>("AvatarId").HasColumnType("INTEGER").HasColumnName("avatar_id");
			b.Property<uint>("UserUid").HasColumnType("INTEGER").HasColumnName("user_uid");
			b.HasKey("Guid");
			b.HasIndex("AvatarId");
			b.ToTable("user_avatar");
		});
		modelBuilder.Entity("relic_profile_item", delegate(EntityTypeBuilder b)
		{
			b.Property<ulong>("relic_item_id").HasColumnType("INTEGER");
			b.Property<int>("profile_id").HasColumnType("INTEGER");
			b.HasKey("relic_item_id", "profile_id");
			b.HasIndex("profile_id");
			b.ToTable("relic_profile_item");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicAffix", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbRelicItem", "Relic").WithMany("Affixes").HasForeignKey("RelicGuid")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Relic");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbRelic", "Relic").WithMany().HasForeignKey("ItemId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Relic");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbUserAvatar", "UserAvatar").WithMany("RelicProfiles").HasForeignKey("AvatarGuid")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("UserAvatar");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicProfileTeamContext", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbRelicProfile", "Profile").WithMany("TeamContexts").HasForeignKey("ProfileId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Profile");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbAvatar", "Avatar").WithMany().HasForeignKey("AvatarId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Avatar");
		});
		modelBuilder.Entity("relic_profile_item", delegate(EntityTypeBuilder b)
		{
			b.HasOne("RelicService.Data.Database.DbRelicProfile", null).WithMany().HasForeignKey("profile_id")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("RelicService.Data.Database.DbRelicItem", null).WithMany().HasForeignKey("relic_item_id")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicItem", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Affixes");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbRelicProfile", delegate(EntityTypeBuilder b)
		{
			b.Navigation("TeamContexts");
		});
		modelBuilder.Entity("RelicService.Data.Database.DbUserAvatar", delegate(EntityTypeBuilder b)
		{
			b.Navigation("RelicProfiles");
		});
	}
}
