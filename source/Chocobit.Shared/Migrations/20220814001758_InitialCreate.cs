using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chocobit.Shared.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelData",
                columns: table => new
                {
                    LevelDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    PlayerDeaths = table.Column<int>(type: "INTEGER", nullable: false),
                    ClearTime = table.Column<long>(type: "INTEGER", nullable: false),
                    WorldRecord = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstClear = table.Column<bool>(type: "INTEGER", nullable: false),
                    TotalGlobalClears = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlobalAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeElapsed = table.Column<long>(type: "INTEGER", nullable: false),
                    Theme = table.Column<string>(type: "TEXT", nullable: true),
                    GameStyle = table.Column<string>(type: "TEXT", nullable: true),
                    Difficulty = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    DateTimeUploaded = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClearConditionMagnitude = table.Column<int>(type: "INTEGER", nullable: true),
                    ClearCondition = table.Column<string>(type: "TEXT", nullable: true),
                    DateTimeStarted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeCleared = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelData", x => x.LevelDataId);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerName = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.PlayerId);
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

            migrationBuilder.DropTable(
                name: "Player");
        }
    }
}
