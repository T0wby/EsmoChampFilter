using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EsmoChamps.Migrations
{
    /// <inheritdoc />
    public partial class AddedChampionStrengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StrengthTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrengthTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChampionStrengths",
                columns: table => new
                {
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    StrengthTitleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampionStrengths", x => new { x.ChampionId, x.StrengthTitleId });
                    table.ForeignKey(
                        name: "FK_ChampionStrengths_Champions_ChampionId",
                        column: x => x.ChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChampionStrengths_StrengthTitles_StrengthTitleId",
                        column: x => x.StrengthTitleId,
                        principalTable: "StrengthTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StrengthTitles",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { 1, "The ability to force a fight to start, usually by closing the gap quickly.", "Engage" },
                    { 2, "The ability to limit enemy movement or actions through stuns, slows or roots.", "Crowd Control" },
                    { 3, "The ability to catch out a single enemy and eliminate them before a teamfight starts.", "Pick Potential" },
                    { 4, "The ability to secure major objectives like Dragon, Baron or Ram.", "Objective Control" },
                    { 5, "Strength in large-scale 5v5 battles involving the entire team.", "Teamfight" },
                    { 6, "The ability to deal significant damage to multiple targets simultaneously.", "Area Damage" },
                    { 7, "Strength in 1v1 combat scenarios against other heroes.", "Dueling" },
                    { 8, "The speed at which the hero can kill jungle camps or minion waves.", "Clear Speed" },
                    { 9, "Strength in small-scale, chaotic fights (e.g. 2v2 or 3v3).", "Skirmishing" },
                    { 10, "The ability to ambush enemies in their lanes to secure kills or advantages.", "Ganking" },
                    { 11, "The ability to control space and deny enemies access to specific areas.", "Zoning" },
                    { 12, "The ability to kill minion waves quickly to defend turrets or push lanes.", "Wave Clear" },
                    { 13, "The ability to pressure enemy turrets and force enemies to defend under them.", "Siege" },
                    { 14, "The ability to protect vulnerable allies by intercepting or disabling enemy divers.", "Peel" },
                    { 15, "The ability to quickly move between lanes to impact other parts of the map.", "Roaming" },
                    { 16, "The ability to apply pressure in a side lane alone while the team fights elsewhere.", "Split Push" },
                    { 17, "The ability to chase down and finish off low-health enemies after a fight.", "Cleanup" },
                    { 18, "The ability to deal damage from a long range without fully committing to a fight.", "Poke" },
                    { 19, "Strength in controlling the early laning phase and suppressing the opponent.", "Lane Dominance" },
                    { 20, "The ability to convert an early lead into an unstoppable advantage.", "Snowball" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChampionStrengths_StrengthTitleId",
                table: "ChampionStrengths",
                column: "StrengthTitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampionStrengths");

            migrationBuilder.DropTable(
                name: "StrengthTitles");
        }
    }
}
