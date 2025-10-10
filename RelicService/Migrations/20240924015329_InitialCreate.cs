using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RelicService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "avatar",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    text_id = table.Column<uint>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    icon_name = table.Column<string>(type: "TEXT", nullable: false),
                    icon_base64 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avatar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relic",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    text_id = table.Column<uint>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    icon_name = table.Column<string>(type: "TEXT", nullable: false),
                    icon_base64 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_avatar",
                columns: table => new
                {
                    guid = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    avatar_id = table.Column<uint>(type: "INTEGER", nullable: false),
                    user_uid = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_avatar", x => x.guid);
                    table.ForeignKey(
                        name: "FK_user_avatar_avatar_avatar_id",
                        column: x => x.avatar_id,
                        principalTable: "avatar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relic_item",
                columns: table => new
                {
                    guid = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    level = table.Column<uint>(type: "INTEGER", nullable: false),
                    main_prop_id = table.Column<uint>(type: "INTEGER", nullable: false),
                    rank_level = table.Column<uint>(type: "INTEGER", nullable: false),
                    equip_type = table.Column<string>(type: "TEXT", nullable: false),
                    main_prop_type = table.Column<string>(type: "TEXT", nullable: false),
                    main_prop_value = table.Column<float>(type: "REAL", nullable: false),
                    item_id = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic_item", x => x.guid);
                    table.ForeignKey(
                        name: "FK_relic_item_relic_item_id",
                        column: x => x.item_id,
                        principalTable: "relic",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relic_profile",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_name = table.Column<string>(type: "TEXT", nullable: false),
                    avatar_guid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    auto_equip = table.Column<bool>(type: "INTEGER", nullable: false),
                    with_scene = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic_profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_relic_profile_user_avatar_avatar_guid",
                        column: x => x.avatar_guid,
                        principalTable: "user_avatar",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relic_affix",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prop_type = table.Column<string>(type: "TEXT", nullable: false),
                    prop_value = table.Column<float>(type: "REAL", nullable: false),
                    relic_guid = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic_affix", x => x.id);
                    table.ForeignKey(
                        name: "FK_relic_affix_relic_item_relic_guid",
                        column: x => x.relic_guid,
                        principalTable: "relic_item",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relic_profile_item",
                columns: table => new
                {
                    relic_item_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic_profile_item", x => new { x.relic_item_id, x.profile_id });
                    table.ForeignKey(
                        name: "FK_relic_profile_item_relic_item_relic_item_id",
                        column: x => x.relic_item_id,
                        principalTable: "relic_item",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_relic_profile_item_relic_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "relic_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relic_profile_team",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    avatar_ids = table.Column<string>(type: "TEXT", nullable: false),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relic_profile_team", x => x.id);
                    table.ForeignKey(
                        name: "FK_relic_profile_team_relic_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "relic_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_relic_affix_relic_guid",
                table: "relic_affix",
                column: "relic_guid");

            migrationBuilder.CreateIndex(
                name: "IX_relic_item_item_id",
                table: "relic_item",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_relic_profile_avatar_guid",
                table: "relic_profile",
                column: "avatar_guid");

            migrationBuilder.CreateIndex(
                name: "IX_relic_profile_item_profile_id",
                table: "relic_profile_item",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_relic_profile_team_profile_id",
                table: "relic_profile_team",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_avatar_avatar_id",
                table: "user_avatar",
                column: "avatar_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "relic_affix");

            migrationBuilder.DropTable(
                name: "relic_profile_item");

            migrationBuilder.DropTable(
                name: "relic_profile_team");

            migrationBuilder.DropTable(
                name: "relic_item");

            migrationBuilder.DropTable(
                name: "relic_profile");

            migrationBuilder.DropTable(
                name: "relic");

            migrationBuilder.DropTable(
                name: "user_avatar");

            migrationBuilder.DropTable(
                name: "avatar");
        }
    }
}
