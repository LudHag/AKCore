using Microsoft.EntityFrameworkCore.Migrations;

namespace AKCore.Migrations
{
    public partial class albumprops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Albums",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Albums",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Albums");
        }
    }
}
