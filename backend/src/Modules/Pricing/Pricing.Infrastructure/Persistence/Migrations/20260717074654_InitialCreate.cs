using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pricing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pricing_schema");

            migrationBuilder.CreateTable(
                name: "Coupons",
                schema: "pricing_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinCartAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    ScopeReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalUsageLimit = table.Column<int>(type: "int", nullable: true),
                    PerUserUsageLimit = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CouponUsages",
                schema: "pricing_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuestCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponUsages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                schema: "pricing_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                schema: "pricing_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "pricing_schema",
                table: "TaxRates",
                columns: new[] { "Id", "Rate", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), 20m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                schema: "pricing_schema",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CouponId",
                schema: "pricing_schema",
                table: "CouponUsages",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CouponId_CustomerId",
                schema: "pricing_schema",
                table: "CouponUsages",
                columns: new[] { "CouponId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CouponId_GuestCustomerId",
                schema: "pricing_schema",
                table: "CouponUsages",
                columns: new[] { "CouponId", "GuestCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_SellableItemId_SellableItemType",
                schema: "pricing_schema",
                table: "Prices",
                columns: new[] { "SellableItemId", "SellableItemType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons",
                schema: "pricing_schema");

            migrationBuilder.DropTable(
                name: "CouponUsages",
                schema: "pricing_schema");

            migrationBuilder.DropTable(
                name: "Prices",
                schema: "pricing_schema");

            migrationBuilder.DropTable(
                name: "TaxRates",
                schema: "pricing_schema");
        }
    }
}
