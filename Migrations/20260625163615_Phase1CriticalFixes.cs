using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetStarterKit.Migrations
{
    /// <inheritdoc />
    public partial class Phase1CriticalFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pilgrims_companies_CompanyId",
                table: "pilgrims");

            migrationBuilder.Sql("UPDATE pilgrims SET CompanyId = 1 WHERE CompanyId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "pilgrims",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "companies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_pilgrims_companies_CompanyId",
                table: "pilgrims",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pilgrims_companies_CompanyId",
                table: "pilgrims");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "companies");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "pilgrims",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_pilgrims_companies_CompanyId",
                table: "pilgrims",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
