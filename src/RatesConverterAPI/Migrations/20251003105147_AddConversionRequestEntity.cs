using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RatesConverterAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConversionRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversionRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ToCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    ConvertedAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    RateSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ClientIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "integer", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_Analytics",
                table: "ConversionRequests",
                columns: new[] { "RequestedAt", "FromCurrency", "ToCurrency", "IsSuccessful" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_CorrelationId",
                table: "ConversionRequests",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_CurrencyPair",
                table: "ConversionRequests",
                columns: new[] { "FromCurrency", "ToCurrency" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_IsSuccessful",
                table: "ConversionRequests",
                column: "IsSuccessful");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_RateSource",
                table: "ConversionRequests",
                column: "RateSource");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRequests_RequestedAt",
                table: "ConversionRequests",
                column: "RequestedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversionRequests");
        }
    }
}
