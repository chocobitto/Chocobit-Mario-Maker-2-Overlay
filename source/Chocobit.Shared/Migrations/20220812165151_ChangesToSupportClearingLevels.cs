using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarioMaker2Overlay.Migrations
{
    public partial class ChangesToSupportClearingLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClearTime",
                table: "LevelData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeCleared",
                table: "LevelData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeStarted",
                table: "LevelData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "FirstClear",
                table: "LevelData",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "LevelData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WorldRecord",
                table: "LevelData",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropColumn(
                name: "ClearTime",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "DateTimeCleared",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "DateTimeStarted",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "FirstClear",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "WorldRecord",
                table: "LevelData");
        }
    }
}
