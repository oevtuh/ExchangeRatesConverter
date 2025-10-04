using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RatesConverterAPI.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCurrencyPairEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_LastUpdated",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "RateSource",
                table: "CurrencyPairs");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "CurrencyPairs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "CurrencyPairs",
                newName: "IsEnabled");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyPairs_IsActive",
                table: "CurrencyPairs",
                newName: "IX_CurrencyPairs_IsEnabled");

            migrationBuilder.InsertData(
                table: "CurrencyPairs",
                columns: new[] { "Id", "CreatedAt", "FromCurrency", "IsEnabled", "Notes", "ToCurrency" },
                values: new object[,]
                {
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USD", true, null, "JPY" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "JPY", true, null, "USD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "CurrencyPairs",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CurrencyPairs",
                newName: "LastUpdated");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyPairs_IsEnabled",
                table: "CurrencyPairs",
                newName: "IX_CurrencyPairs_IsActive");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "CurrencyPairs",
                type: "numeric(18,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RateSource",
                table: "CurrencyPairs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 0.85m, "Manual" });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 1.18m, "Manual" });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 0.73m, "Manual" });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 1.37m, "Manual" });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 0.86m, "Manual" });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ExchangeRate", "RateSource" },
                values: new object[] { 1.16m, "Manual" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_LastUpdated",
                table: "CurrencyPairs",
                column: "LastUpdated");
        }
    }
}
