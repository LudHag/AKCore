using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEventMemberSignup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SignUps_EventId_PersonId",
                table: "SignUps",
                columns: new[] { "EventId", "PersonId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_SignUps_EventId",
                table: "SignUps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SignUps_EventId",
                table: "SignUps",
                column: "EventId");

            migrationBuilder.DropIndex(
                name: "IX_SignUps_EventId_PersonId",
                table: "SignUps");
        }
    }
}
