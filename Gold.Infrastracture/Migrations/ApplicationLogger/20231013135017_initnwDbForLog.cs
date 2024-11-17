using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gold.Infrastracture.Migrations.ApplicationLogger
{
    public partial class initnwDbForLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "IsConfirm",
            //    schema: "dbo",
            //    table: "CustomerPayment");

            //migrationBuilder.AddColumn<long>(
            //    name: "ReturnedAmount",
            //    schema: "dbo",
            //    table: "Document",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "ImageName",
            //    schema: "dbo",
            //    table: "CustomerPayment",
            //    type: "varchar(100)",
            //    maxLength: 100,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)");

            //migrationBuilder.AddColumn<string>(
            //    name: "AdminDescription",
            //    schema: "dbo",
            //    table: "CustomerPayment",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte>(
            //    name: "ConfirmStatus",
            //    schema: "dbo",
            //    table: "CustomerPayment",
            //    type: "tinyint",
            //    nullable: false,
            //    defaultValue: (byte)0);

            //migrationBuilder.AddColumn<long>(
            //    name: "PayAmount",
            //    schema: "dbo",
            //    table: "CustomerPayment",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "PayDate",
            //    schema: "dbo",
            //    table: "CustomerPayment",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte>(
            //    name: "Type",
            //    schema: "dbo",
            //    table: "CustomerMessage",
            //    type: "tinyint",
            //    nullable: false,
            //    defaultValue: (byte)0);

            //migrationBuilder.AlterColumn<string>(
            //    name: "NationalCode",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "varchar(50)",
            //    maxLength: 50,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "varchar(10)",
            //    oldMaxLength: 10);

            //migrationBuilder.AddColumn<byte>(
            //    name: "BirthDateDay",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "tinyint",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte>(
            //    name: "BirthDateMonth",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "tinyint",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "BirthDateYear",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte>(
            //    name: "Gender",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "tinyint",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte>(
            //    name: "Nationality",
            //    schema: "dbo",
            //    table: "Customer",
            //    type: "tinyint",
            //    nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "BankCardNo",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
            //        Owner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        OrderNo = table.Column<int>(type: "int", nullable: false),
            //        CustomerId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BankCardNo", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_BankCardNo_Customer_CustomerId",
            //            column: x => x.CustomerId,
            //            principalSchema: "dbo",
            //            principalTable: "Customer",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProfileImage",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ImageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CustomerId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProfileImage", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ProfileImage_Customer_CustomerId",
            //            column: x => x.CustomerId,
            //            principalSchema: "dbo",
            //            principalTable: "Customer",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.UpdateData(
            //    schema: "dbo",
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: 100000,
            //    columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
            //    values: new object[] { "I8HMkxPuoMlA/opIA++uOQ==", "fe890d17-26f6-4767-980d-2a3a9e464627", new DateTime(2023, 10, 13, 17, 20, 17, 306, DateTimeKind.Local).AddTicks(4162), new Guid("38a2bd58-0363-42ad-b05f-bd70aaf7f07a") });

            //migrationBuilder.UpdateData(
            //    schema: "dbo",
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: 100001,
            //    columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
            //    values: new object[] { "g76tBME8Lz0DQt5++GSXBw==", "d7010d89-cb10-4f4e-b4b1-cbb0f874eafc", new DateTime(2023, 10, 13, 17, 20, 17, 307, DateTimeKind.Local).AddTicks(2059), new Guid("8062bf27-e72b-42fd-97a8-38198291cf88") });

            //migrationBuilder.CreateIndex(
            //    name: "IX_BankCardNo_CustomerId",
            //    schema: "dbo",
            //    table: "BankCardNo",
            //    column: "CustomerId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProfileImage_CustomerId",
            //    schema: "dbo",
            //    table: "ProfileImage",
            //    column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.DropTable(
        //        name: "BankCardNo",
        //        schema: "dbo");

        //    migrationBuilder.DropTable(
        //        name: "ProfileImage",
        //        schema: "dbo");

        //    migrationBuilder.DropColumn(
        //        name: "ReturnedAmount",
        //        schema: "dbo",
        //        table: "Document");

        //    migrationBuilder.DropColumn(
        //        name: "AdminDescription",
        //        schema: "dbo",
        //        table: "CustomerPayment");

        //    migrationBuilder.DropColumn(
        //        name: "ConfirmStatus",
        //        schema: "dbo",
        //        table: "CustomerPayment");

        //    migrationBuilder.DropColumn(
        //        name: "PayAmount",
        //        schema: "dbo",
        //        table: "CustomerPayment");

        //    migrationBuilder.DropColumn(
        //        name: "PayDate",
        //        schema: "dbo",
        //        table: "CustomerPayment");

        //    migrationBuilder.DropColumn(
        //        name: "Type",
        //        schema: "dbo",
        //        table: "CustomerMessage");

        //    migrationBuilder.DropColumn(
        //        name: "BirthDateDay",
        //        schema: "dbo",
        //        table: "Customer");

        //    migrationBuilder.DropColumn(
        //        name: "BirthDateMonth",
        //        schema: "dbo",
        //        table: "Customer");

        //    migrationBuilder.DropColumn(
        //        name: "BirthDateYear",
        //        schema: "dbo",
        //        table: "Customer");

        //    migrationBuilder.DropColumn(
        //        name: "Gender",
        //        schema: "dbo",
        //        table: "Customer");

        //    migrationBuilder.DropColumn(
        //        name: "Nationality",
        //        schema: "dbo",
        //        table: "Customer");

        //    migrationBuilder.AlterColumn<string>(
        //        name: "ImageName",
        //        schema: "dbo",
        //        table: "CustomerPayment",
        //        type: "nvarchar(max)",
        //        nullable: false,
        //        defaultValue: "",
        //        oldClrType: typeof(string),
        //        oldType: "varchar(100)",
        //        oldMaxLength: 100,
        //        oldNullable: true);

        //    migrationBuilder.AddColumn<bool>(
        //        name: "IsConfirm",
        //        schema: "dbo",
        //        table: "CustomerPayment",
        //        type: "bit",
        //        nullable: false,
        //        defaultValue: false);

        //    migrationBuilder.AlterColumn<string>(
        //        name: "NationalCode",
        //        schema: "dbo",
        //        table: "Customer",
        //        type: "varchar(10)",
        //        maxLength: 10,
        //        nullable: false,
        //        oldClrType: typeof(string),
        //        oldType: "varchar(50)",
        //        oldMaxLength: 50);

        //    migrationBuilder.UpdateData(
        //        schema: "dbo",
        //        table: "User",
        //        keyColumn: "Id",
        //        keyValue: 100000,
        //        columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
        //        values: new object[] { "HpxDLJAkYsx9Cg0Gc8JaZw==", "226adde5-7b16-4619-82ed-3ad9c896455e", new DateTime(2023, 5, 9, 12, 20, 43, 876, DateTimeKind.Local).AddTicks(9120), new Guid("7c03e1ce-fe0a-4e20-a663-1b7223d3255f") });

        //    migrationBuilder.UpdateData(
        //        schema: "dbo",
        //        table: "User",
        //        keyColumn: "Id",
        //        keyValue: 100001,
        //        columns: new[] { "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp" },
        //        values: new object[] { "srGQD27h9PWuj9VSKNLrSg==", "bb1ec1bb-dabb-4de3-903a-3b68a925c4d0", new DateTime(2023, 5, 9, 12, 20, 43, 877, DateTimeKind.Local).AddTicks(6472), new Guid("7e8d2902-c78d-46f2-bafa-76592ec1eaf3") });
        }
    }
}
