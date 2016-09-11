using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AKCore.Migrations
{
    public partial class extraeventvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalDescription",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalDescription",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "Events");
        }
    }
}
