using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rozkaz.Migrations
{
    public partial class AddedUnitModelToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UnitUid",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UnitModel",
                columns: table => new
                {
                    Uid = table.Column<Guid>(nullable: false),
                    NameFirstLine = table.Column<string>(nullable: true),
                    NameSecondLine = table.Column<string>(nullable: true),
                    SubtextLines = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitModel", x => x.Uid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UnitUid",
                table: "Users",
                column: "UnitUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UnitModel_UnitUid",
                table: "Users",
                column: "UnitUid",
                principalTable: "UnitModel",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UnitModel_UnitUid",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UnitModel");

            migrationBuilder.DropIndex(
                name: "IX_Users_UnitUid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnitUid",
                table: "Users");
        }
    }
}
