using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gold.Infrastracture.Migrations
{
    public partial class initDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AdminMenuGroups",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderNo = table.Column<byte>(type: "tinyint", nullable: false),
                    IconName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenuGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollateralTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasInstallmentSale = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsTemps",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialNetworks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialNetworks", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "SystemActivityLog",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //        StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Source = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
            //        RaiseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ExtraData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SystemActivityLog", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecurityStamp = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WrongPasswordCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminMenus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderNo = table.Column<byte>(type: "tinyint", nullable: false),
                    IconName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ControllerName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ActionName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    AdminMenuGroupID = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminMenus_AdminMenuGroups_AdminMenuGroupID",
                        column: x => x.AdminMenuGroupID,
                        principalSchema: "dbo",
                        principalTable: "AdminMenuGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NationalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SanaCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "dbo",
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "dbo",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Managers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductRegisterPerHourCount = table.Column<byte>(type: "tinyint", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasAccessToRegisterLoan = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GalleryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sellers_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalSchema: "dbo",
                        principalTable: "Galleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sellers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminActivities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AdminMenuID = table.Column<byte>(type: "tinyint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityType = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminActivities_AdminMenus_AdminMenuID",
                        column: x => x.AdminMenuID,
                        principalSchema: "dbo",
                        principalTable: "AdminMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminActivities_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAccesses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    AdminMenuId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionAccesses_AdminMenus_AdminMenuId",
                        column: x => x.AdminMenuId,
                        principalSchema: "dbo",
                        principalTable: "AdminMenus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PermissionAccesses_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "dbo",
                        principalTable: "Permissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EditInformationRequests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ImageName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditInformationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EditInformationRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EssentialTels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationShip = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    Tel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EssentialTels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EssentialTels_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    GalleryId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    DocumentNo = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "date", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrepaymentAmount = table.Column<long>(type: "bigint", nullable: false),
                    RemainAmount = table.Column<long>(type: "bigint", nullable: false),
                    InstallmentCount = table.Column<byte>(type: "tinyint", nullable: false),
                    InstallmentAmount = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    DayOfMonth = table.Column<byte>(type: "tinyint", nullable: false),
                    AdminDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DelayAmount = table.Column<long>(type: "bigint", nullable: true),
                    DiscountAmount = table.Column<long>(type: "bigint", nullable: false),
                    SettleDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "date", nullable: true),
                    SettleRegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalSchema: "dbo",
                        principalTable: "Galleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalSchema: "dbo",
                        principalTable: "Sellers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Collaterals",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollateralTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaterals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collaterals_CollateralTypes_CollateralTypeId",
                        column: x => x.CollateralTypeId,
                        principalSchema: "dbo",
                        principalTable: "CollateralTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Collaterals_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerPayments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirm = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Installments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Number = table.Column<byte>(type: "tinyint", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    DelayDays = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Installments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerMessages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    installmentId = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerMessages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerMessages_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerMessages_Installments_installmentId",
                        column: x => x.installmentId,
                        principalSchema: "dbo",
                        principalTable: "Installments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstallmentId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerPaymentId = table.Column<long>(type: "bigint", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_CustomerPayments_CustomerPaymentId",
                        column: x => x.CustomerPaymentId,
                        principalSchema: "dbo",
                        principalTable: "CustomerPayments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payments_Installments_InstallmentId",
                        column: x => x.InstallmentId,
                        principalSchema: "dbo",
                        principalTable: "Installments",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "CollateralTypes",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "سکه" },
                    { 2, "گرم" },
                    { 3, "سند" },
                    { 4, "چک" },
                    { 5, "گالری" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "IsActive", "IsLocked", "LastLoginDate", "LastModifiedDate", "LastPasswordChangeDate", "LockDate", "Mobile", "PasswordHash", "PasswordSalt", "RegisterDate", "SecurityStamp", "UserName", "UserType" },
                values: new object[,]
                {
                    { 100000, null, "آراپندار", true, false, null, null, null, null, "", "ssDhi4t9/j1aedeLx28C2g==", "61b78f20-c451-4bb2-b5dc-22ac30804331", new DateTime(2023, 5, 9, 12, 16, 12, 824, DateTimeKind.Local).AddTicks(1521), new Guid("6266eab6-9908-46ea-96bf-de2d1981b982"), "pendarAdmin", (byte)3 },
                    { 100001, null, "مدیر سیستم", true, false, null, null, null, null, "", "hlTfAQtAZJVjCwR0yHr1DA==", "82885a65-99ac-4523-9448-dde2e7f4120d", new DateTime(2023, 5, 9, 12, 16, 12, 824, DateTimeKind.Local).AddTicks(8936), new Guid("b8e684dd-f965-4d0a-801e-e222e49ecb8e"), "siteAdmin", (byte)3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivities_AdminMenuID",
                schema: "dbo",
                table: "AdminActivities",
                column: "AdminMenuID");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivities_UserId",
                schema: "dbo",
                table: "AdminActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminMenus_AdminMenuGroupID",
                schema: "dbo",
                table: "AdminMenus",
                column: "AdminMenuGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Collaterals_CollateralTypeId",
                schema: "dbo",
                table: "Collaterals",
                column: "CollateralTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Collaterals_DocumentId",
                schema: "dbo",
                table: "Collaterals",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessages_CustomerId",
                schema: "dbo",
                table: "CustomerMessages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessages_DocumentId",
                schema: "dbo",
                table: "CustomerMessages",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessages_installmentId",
                schema: "dbo",
                table: "CustomerMessages",
                column: "installmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_DocumentId",
                schema: "dbo",
                table: "CustomerPayments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CityId",
                schema: "dbo",
                table: "Customers",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                schema: "dbo",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId",
                schema: "dbo",
                table: "Documents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_GalleryId",
                schema: "dbo",
                table: "Documents",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SellerId",
                schema: "dbo",
                table: "Documents",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_EditInformationRequests_CustomerId",
                schema: "dbo",
                table: "EditInformationRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EssentialTels_CustomerId",
                schema: "dbo",
                table: "EssentialTels",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Installments_DocumentId",
                schema: "dbo",
                table: "Installments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_PermissionId",
                schema: "dbo",
                table: "Managers",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_UserId",
                schema: "dbo",
                table: "Managers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerPaymentId",
                schema: "dbo",
                table: "Payments",
                column: "CustomerPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InstallmentId",
                schema: "dbo",
                table: "Payments",
                column: "InstallmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAccesses_AdminMenuId",
                schema: "dbo",
                table: "PermissionAccesses",
                column: "AdminMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAccesses_PermissionId",
                schema: "dbo",
                table: "PermissionAccesses",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_GalleryId",
                schema: "dbo",
                table: "Sellers",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_UserId",
                schema: "dbo",
                table: "Sellers",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminActivities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Collaterals",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerMessages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EditInformationRequests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EssentialTels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Managers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PermissionAccesses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SmsTemps",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SocialNetworks",
                schema: "dbo");

            //migrationBuilder.DropTable(
            //    name: "SystemActivityLog",
            //    schema: "dbo");

            migrationBuilder.DropTable(
                name: "CollateralTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerPayments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Installments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AdminMenus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AdminMenuGroups",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Galleries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");
        }
    }
}
