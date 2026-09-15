using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKCore.Migrations
{
    /// <inheritdoc />
    public partial class AddMobileNotificationDeviceDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstallationId",
                table: "MobileNotificationDeliveries",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MobileNotificationDeliveries_UserId_EventId_InstallationId",
                table: "MobileNotificationDeliveries",
                columns: new[] { "UserId", "EventId", "InstallationId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_MobileNotificationDeliveries_UserId_EventId",
                table: "MobileNotificationDeliveries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM `MobileNotificationDeliveries`;");

            migrationBuilder.CreateIndex(
                name: "IX_MobileNotificationDeliveries_UserId_EventId",
                table: "MobileNotificationDeliveries",
                columns: new[] { "UserId", "EventId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_MobileNotificationDeliveries_UserId_EventId_InstallationId",
                table: "MobileNotificationDeliveries");

            migrationBuilder.DropColumn(
                name: "InstallationId",
                table: "MobileNotificationDeliveries");
        }
    }
}
