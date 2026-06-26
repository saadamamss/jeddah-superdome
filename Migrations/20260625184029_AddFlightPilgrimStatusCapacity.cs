using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetStarterKit.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightPilgrimStatusCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "flights",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "flights",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "flight_pilgrims",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    PilgrimId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flight_pilgrims", x => new { x.FlightId, x.PilgrimId });
                    table.ForeignKey(
                        name: "FK_flight_pilgrims_flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_flight_pilgrims_pilgrims_PilgrimId",
                        column: x => x.PilgrimId,
                        principalTable: "pilgrims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_flights_FlightNumber",
                table: "flights",
                column: "FlightNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_flight_pilgrims_PilgrimId",
                table: "flight_pilgrims",
                column: "PilgrimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flight_pilgrims");

            migrationBuilder.DropIndex(
                name: "IX_flights_FlightNumber",
                table: "flights");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "flights");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "flights");
        }
    }
}
