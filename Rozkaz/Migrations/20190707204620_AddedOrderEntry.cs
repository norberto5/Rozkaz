using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rozkaz.Migrations
{
    public partial class AddedOrderEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderEntry",
                columns: table => new
                {
                    Uid = table.Column<Guid>(nullable: false),
                    Order = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEntry", x => x.Uid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderEntry");
        }
    }
}
