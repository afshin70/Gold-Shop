using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gold.Infrastracture.Migrations
{
    public partial class updateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 100000,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "sPL/o8baGtxT6lkfRRAhxQ==", "54f97899-3b0e-4e5d-aac9-12168fb726fe", new DateTime(2023, 5, 9, 12, 20, 12, 408, DateTimeKind.Local).AddTicks(1461), new Guid("edee4204-f9fa-4285-ae13-8d71bf01c544") });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 100001,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "FGnP5vpYRzPRRiGISPUYZw==", "a0e3f8e1-abf9-48d2-b419-d340451335cd", new DateTime(2023, 5, 9, 12, 20, 12, 409, DateTimeKind.Local).AddTicks(757), new Guid("c5ea1a4e-0856-40e5-8a83-2311b86811fb") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 100000,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "ssDhi4t9/j1aedeLx28C2g==", "61b78f20-c451-4bb2-b5dc-22ac30804331", new DateTime(2023, 5, 9, 12, 16, 12, 824, DateTimeKind.Local).AddTicks(1521), new Guid("6266eab6-9908-46ea-96bf-de2d1981b982") });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 100001,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "hlTfAQtAZJVjCwR0yHr1DA==", "82885a65-99ac-4523-9448-dde2e7f4120d", new DateTime(2023, 5, 9, 12, 16, 12, 824, DateTimeKind.Local).AddTicks(8936), new Guid("b8e684dd-f965-4d0a-801e-e222e49ecb8e") });
        }
    }
}
