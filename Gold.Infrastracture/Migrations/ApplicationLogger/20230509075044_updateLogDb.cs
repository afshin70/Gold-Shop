using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gold.Infrastracture.Migrations.ApplicationLogger
{
    public partial class updateLogDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "User",
                keyColumn: "Id",
                keyValue: 100000,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "HpxDLJAkYsx9Cg0Gc8JaZw==", "226adde5-7b16-4619-82ed-3ad9c896455e", new DateTime(2023, 5, 9, 12, 20, 43, 876, DateTimeKind.Local).AddTicks(9120), new Guid("7c03e1ce-fe0a-4e20-a663-1b7223d3255f") });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "User",
                keyColumn: "Id",
                keyValue: 100001,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "srGQD27h9PWuj9VSKNLrSg==", "bb1ec1bb-dabb-4de3-903a-3b68a925c4d0", new DateTime(2023, 5, 9, 12, 20, 43, 877, DateTimeKind.Local).AddTicks(6472), new Guid("7e8d2902-c78d-46f2-bafa-76592ec1eaf3") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "User",
                keyColumn: "Id",
                keyValue: 100000,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "82dWtIEPh15vJ9F2GcAbDw==", "c66a7c2b-a79f-4aa1-b27a-87bca740e514", new DateTime(2023, 5, 9, 12, 17, 51, 52, DateTimeKind.Local).AddTicks(1141), new Guid("7a389637-71a4-47af-afed-d327911a20d5") });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "User",
                keyColumn: "Id",
                keyValue: 100001,
                columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
                values: new object[] { "OaLj53aur6sI6ctK1T2QlA==", "78a5dc44-8e84-4789-82be-b2569577f19d", new DateTime(2023, 5, 9, 12, 17, 51, 52, DateTimeKind.Local).AddTicks(8434), new Guid("8776afe3-d227-4c22-9dda-4c5e3d02933d") });
        }
    }
}
