using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chocobit.Shared.Migrations
{
    public partial class ReviseUniqueConstraintAndIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_LevelData_Code", "LevelData");

            migrationBuilder.CreateIndex(
                name: "IX_LevelData_PlayerId_Code",
                table: "LevelData",
                columns: new[] { "PlayerId", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_LevelData_PlayerId_Code", "LevelData");

            migrationBuilder.CreateIndex(
                name: "IX_LevelData_Code",
                table: "LevelData",
                column: "Code",
                unique: true);
        }
    }
}
