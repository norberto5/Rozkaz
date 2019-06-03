using Microsoft.EntityFrameworkCore.Migrations;

namespace Rozkaz.Migrations
{
    public partial class AddedMoreUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Users");
        }
    }
}
