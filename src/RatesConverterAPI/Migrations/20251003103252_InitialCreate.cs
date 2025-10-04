using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RatesConverterAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ToCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    RateSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyPairs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CurrencyPairs",
                columns: new[] { "Id", "ExchangeRate", "FromCurrency", "IsActive", "LastUpdated", "Notes", "RateSource", "ToCurrency" },
                values: new object[,]
                {
                    { 1, 0.85m, "USD", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "EUR" },
                    { 2, 1.18m, "EUR", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "USD" },
                    { 3, 0.73m, "USD", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "GBP" },
                    { 4, 1.37m, "GBP", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "USD" },
                    { 5, 0.86m, "EUR", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "GBP" },
                    { 6, 1.16m, "GBP", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manual", "EUR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_FromTo_Unique",
                table: "CurrencyPairs",
                columns: new[] { "FromCurrency", "ToCurrency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_IsActive",
                table: "CurrencyPairs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_LastUpdated",
                table: "CurrencyPairs",
                column: "LastUpdated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyPairs");
        }
    }
}
