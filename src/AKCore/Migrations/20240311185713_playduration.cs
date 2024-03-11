using Microsoft.EntityFrameworkCore.Migrations;

namespace AKCore.Migrations
{
    public partial class AddPlayDurationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayDuration",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayDuration",
                table: "Events");
        }
    }
}