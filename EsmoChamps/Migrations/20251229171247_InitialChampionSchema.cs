using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EsmoChamps.Migrations
{
    /// <inheritdoc />
    public partial class InitialChampionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChampTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RangeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Champions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    RangeTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChampTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    MechanicsMin = table.Column<int>(type: "INTEGER", nullable: false),
                    MechanicsMax = table.Column<int>(type: "INTEGER", nullable: false),
                    MacroMin = table.Column<int>(type: "INTEGER", nullable: false),
                    MacroMax = table.Column<int>(type: "INTEGER", nullable: false),
                    TacticalMin = table.Column<int>(type: "INTEGER", nullable: false),
                    TacticalMax = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Champions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Champions_ChampTypes_ChampTypeId",
                        column: x => x.ChampTypeId,
                        principalTable: "ChampTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Champions_RangeTypes_RangeTypeId",
                        column: x => x.RangeTypeId,
                        principalTable: "RangeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Champions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChampTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Artillery Mage" },
                    { 2, "Burst Mage" },
                    { 3, "Bruiser" },
                    { 4, "Defensive Tank" },
                    { 5, "Offensive Tank" },
                    { 6, "Assassin" },
                    { 7, "Battle Mage" },
                    { 8, "Marksman" },
                    { 9, "Enchanter" },
                    { 10, "Catcher" },
                    { 11, "Diver" },
                    { 12, "Duelist" }
                });

            migrationBuilder.InsertData(
                table: "RangeTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Melee" },
                    { 2, "Ranged" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Top Lane" },
                    { 2, "Jungle" },
                    { 3, "Mid Lane" },
                    { 4, "Bot Lane" },
                    { 5, "Support" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Champions_ChampTypeId",
                table: "Champions",
                column: "ChampTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Champions_RangeTypeId",
                table: "Champions",
                column: "RangeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Champions_RoleId",
                table: "Champions",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Champions");

            migrationBuilder.DropTable(
                name: "ChampTypes");

            migrationBuilder.DropTable(
                name: "RangeTypes");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
