using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsmoChamps.Migrations
{
    /// <inheritdoc />
    public partial class AddPowerCurveProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PowerCurveEnd",
                table: "Champions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PowerCurveMid",
                table: "Champions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PowerCurveStart",
                table: "Champions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PowerCurveEnd",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "PowerCurveMid",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "PowerCurveStart",
                table: "Champions");
        }
    }
}
