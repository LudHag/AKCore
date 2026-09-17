using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueMobileSessionRefreshTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MobileSessions_RefreshTokenHash",
                table: "MobileSessions",
                column: "RefreshTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MobileSessions_RefreshTokenHash",
                table: "MobileSessions");
        }
    }
}
