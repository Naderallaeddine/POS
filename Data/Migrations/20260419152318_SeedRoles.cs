using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace POS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df001", "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df001", "Admin", "ADMIN" },
                    { "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df002", "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df002", "Cashier", "CASHIER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df001");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df002");
        }
    }
}
