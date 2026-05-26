using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankingAPI.Migrations
{
    /// <inheritdoc />
    public partial class statementadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_ToAccountNumber",
                table: "Transactions",
                newName: "IX_Transactions_ToAccountNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_FromAccountNumber",
                table: "Transactions",
                newName: "IX_Transactions_FromAccountNumber");

            migrationBuilder.CreateTable(
                name: "AccountStatements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    Debit = table.Column<float>(type: "real", nullable: false),
                    Credit = table.Column<float>(type: "real", nullable: false),
                    Balance = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountStatements");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_ToAccountNumber",
                table: "Transaction",
                newName: "IX_Transaction_ToAccountNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_FromAccountNumber",
                table: "Transaction",
                newName: "IX_Transaction_FromAccountNumber");
        }
    }
}
