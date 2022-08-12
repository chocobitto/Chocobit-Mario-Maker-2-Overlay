using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarioMaker2Overlay.Migrations
{
    public partial class AddExtraColumnsToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClearCondition",
                table: "LevelData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClearConditionMagnitude",
                table: "LevelData",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeUploaded",
                table: "LevelData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "LevelData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameStyle",
                table: "LevelData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "LevelData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "LevelData",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClearCondition",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "ClearConditionMagnitude",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "DateTimeUploaded",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "GameStyle",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "LevelData");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "LevelData");
        }
    }
}
