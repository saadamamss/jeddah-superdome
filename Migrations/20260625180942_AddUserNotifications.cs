using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetStarterKit.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SentByUserId",
                table: "notifications",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TargetCompanyId",
                table: "notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_notifications_notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_SentByUserId",
                table: "notifications",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_TargetCompanyId",
                table: "notifications",
                column: "TargetCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_NotificationId",
                table: "user_notifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_UserId_NotificationId",
                table: "user_notifications",
                columns: new[] { "UserId", "NotificationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_companies_TargetCompanyId",
                table: "notifications",
                column: "TargetCompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_SentByUserId",
                table: "notifications",
                column: "SentByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_companies_TargetCompanyId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_SentByUserId",
                table: "notifications");

            migrationBuilder.DropTable(
                name: "user_notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_SentByUserId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_TargetCompanyId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "SentByUserId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "TargetCompanyId",
                table: "notifications");
        }
    }
}
