using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "74c1eab8-d12d-4e7e-a1f0-27a1e24a0636", "813d2e28-0af6-48a3-a637-99f43de5cda8", "Administrator", "ADMINISTRATOR" },
                    { "c38686a8-1151-4218-9fe1-ff35add08542", "e283451f-9643-4172-9a9c-e397ed340328", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74c1eab8-d12d-4e7e-a1f0-27a1e24a0636");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c38686a8-1151-4218-9fe1-ff35add08542");
        }
    }
}
