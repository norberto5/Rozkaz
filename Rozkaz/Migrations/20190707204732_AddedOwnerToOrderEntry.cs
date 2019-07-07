using Microsoft.EntityFrameworkCore.Migrations;

namespace Rozkaz.Migrations
{
    public partial class AddedOwnerToOrderEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "OrderEntry",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderEntry_OwnerId",
                table: "OrderEntry",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderEntry_Users_OwnerId",
                table: "OrderEntry",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderEntry_Users_OwnerId",
                table: "OrderEntry");

            migrationBuilder.DropIndex(
                name: "IX_OrderEntry_OwnerId",
                table: "OrderEntry");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "OrderEntry");
        }
    }
}
