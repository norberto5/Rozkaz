using Microsoft.EntityFrameworkCore.Migrations;

namespace Rozkaz.Migrations
{
    public partial class RenamedOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderEntry_Users_OwnerId",
                table: "OrderEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderEntry",
                table: "OrderEntry");

            migrationBuilder.RenameTable(
                name: "OrderEntry",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_OrderEntry_OwnerId",
                table: "Orders",
                newName: "IX_Orders_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_OwnerId",
                table: "Orders",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_OwnerId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "OrderEntry");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OwnerId",
                table: "OrderEntry",
                newName: "IX_OrderEntry_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderEntry",
                table: "OrderEntry",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderEntry_Users_OwnerId",
                table: "OrderEntry",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
