using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImproveMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OrderItem",
                type: "BLOB",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Order",
                type: "BLOB",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_LastName",
                table: "Person",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Title",
                table: "Book",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Person_LastName",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Book_Title",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Order");
        }
    }
}
