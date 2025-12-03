using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClearBank.DeveloperTest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0.00m),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowedPaymentSchemes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountNumber);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountNumber", "AllowedPaymentSchemes", "Balance", "Status" },
                values: new object[,]
                {
                    { "08845741", "0", 32987.65m, "Live" },
                    { "09773439", "Chaps", 3109.87m, "Live" },
                    { "14653248", "FasterPayments", -250.50m, "InboundPaymentsOnly" },
                    { "23992701", "5", 89012.34m, "Live" },
                    { "31022756", "6", 56789.01m, "Live" },
                    { "36946349", "Bacs", -750.25m, "InboundPaymentsOnly" },
                    { "45302248", "Bacs", 21098.76m, "Live" },
                    { "55694617", "7", 43210.98m, "Live" },
                    { "57047920", "3", 2678.90m, "Live" },
                    { "65015197", "3", 8432.10m, "Disabled" },
                    { "74000307", "7", -1500.75m, "InboundPaymentsOnly" },
                    { "94202716", "FasterPayments", 63254.88m, "Live" },
                    { "99636544", "7", 58327.56m, "Disabled" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
