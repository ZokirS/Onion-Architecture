using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalUserFiledsForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74c1eab8-d12d-4e7e-a1f0-27a1e24a0636");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c38686a8-1151-4218-9fe1-ff35add08542");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "94d58efb-738c-4afe-84ca-18744dd52ecf", "254cf923-142c-41f8-a325-b1c7f3b3fdeb", "Administrator", "ADMINISTRATOR" },
                    { "e90d58c5-4489-4fd1-88e0-d1455d01e2ea", "9d011697-2a9b-4eef-8a53-272cbb21bf62", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94d58efb-738c-4afe-84ca-18744dd52ecf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e90d58c5-4489-4fd1-88e0-d1455d01e2ea");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "74c1eab8-d12d-4e7e-a1f0-27a1e24a0636", "813d2e28-0af6-48a3-a637-99f43de5cda8", "Administrator", "ADMINISTRATOR" },
                    { "c38686a8-1151-4218-9fe1-ff35add08542", "e283451f-9643-4172-9a9c-e397ed340328", "Manager", "MANAGER" }
                });
        }
    }
}
