using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thunders.TechTest.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HourlyCityReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "varchar(450)", nullable: false),
                    State = table.Column<string>(type: "varchar(2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalVehicles = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourlyCityReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyPlazaReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Plaza = table.Column<string>(type: "varchar(450)", nullable: false),
                    City = table.Column<string>(type: "varchar(450)", nullable: false),
                    State = table.Column<string>(type: "varchar(2)", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyPlazaReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TollUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    LicensePlate = table.Column<string>(type: "text", nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Plaza = table.Column<string>(type: "varchar(450)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "varchar(450)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "varchar(2)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TollUsages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypeReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Plaza = table.Column<string>(type: "varchar(450)", nullable: false),
                    City = table.Column<string>(type: "varchar(450)", nullable: false),
                    State = table.Column<string>(type: "varchar(2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    VehicleCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypeReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HourlyCityReports_City_Date_Hour",
                table: "HourlyCityReports",
                columns: new[] { "City", "Date", "Hour" });

            migrationBuilder.CreateIndex(
                name: "IX_HourlyCityReports_State",
                table: "HourlyCityReports",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPlazaReports_Year_Month_Rank",
                table: "MonthlyPlazaReports",
                columns: new[] { "Year", "Month", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_TollUsages_City",
                table: "TollUsages",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_TollUsages_CreatedAt",
                table: "TollUsages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TollUsages_Plaza",
                table: "TollUsages",
                column: "Plaza");

            migrationBuilder.CreateIndex(
                name: "IX_TollUsages_State",
                table: "TollUsages",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypeReports_Plaza_Date_VehicleType",
                table: "VehicleTypeReports",
                columns: new[] { "Plaza", "Date", "VehicleType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HourlyCityReports");

            migrationBuilder.DropTable(
                name: "MonthlyPlazaReports");

            migrationBuilder.DropTable(
                name: "TollUsages");

            migrationBuilder.DropTable(
                name: "VehicleTypeReports");
        }
    }
}
