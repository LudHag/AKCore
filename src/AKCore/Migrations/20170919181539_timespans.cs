using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace AKCore.Migrations
{
    public partial class timespans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "HalanTime",
                table: "Events",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartsTime",
                table: "Events",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ThereTime",
                table: "Events",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HalanTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StartsTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ThereTime",
                table: "Events");
        }
    }
}
