using Microsoft.EntityFrameworkCore.Migrations;

namespace AKCore.Migrations
{
    public partial class medalandloggedout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LoggedOut",
                table: "Pages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Medal",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggedOut",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Medal",
                table: "AspNetUsers");
        }
    }
}
