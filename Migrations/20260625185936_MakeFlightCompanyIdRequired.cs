using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetStarterKit.Migrations
{
    /// <inheritdoc />
    public partial class MakeFlightCompanyIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flights_companies_CompanyId",
                table: "flights");

            // Backfill existing NULL CompanyId with the first company's ID
            migrationBuilder.Sql(@"
                UPDATE flights
                SET CompanyId = (SELECT Id FROM companies ORDER BY Id ASC LIMIT 1)
                WHERE CompanyId IS NULL
            ");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "flights",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_flights_companies_CompanyId",
                table: "flights",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flights_companies_CompanyId",
                table: "flights");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "flights",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_flights_companies_CompanyId",
                table: "flights",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
