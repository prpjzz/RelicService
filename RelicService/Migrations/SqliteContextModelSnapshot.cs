using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelicService.Data.Database;

namespace RelicService.Migrations;

[DbContext(typeof(SqliteContext))]
internal class SqliteContextModelSnapshot : ModelSnapshot
{
	protected override void BuildModel(ModelBuilder modelBuilder)
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
