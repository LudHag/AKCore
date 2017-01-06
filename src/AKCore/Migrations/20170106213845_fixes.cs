using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AKCore.Migrations
{
    public partial class fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Recruits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Recruits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Recruits");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Recruits");
        }
    }
}
