using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarioMaker2Overlay.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelData",
                columns: table => new
                {
                    LevelDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    PlayerDeaths = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlobalClears = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlobalAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeElapsed = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelData", x => x.LevelDataId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LevelData_Code",
                table: "LevelData",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelData");
        }
    }
}
