using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory_schema");

            migrationBuilder.CreateTable(
                name: "StockItems",
                schema: "inventory_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockReservations",
                schema: "inventory_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockReservations_StockItems_StockItemId",
                        column: x => x.StockItemId,
                        principalSchema: "inventory_schema",
                        principalTable: "StockItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_SellableItemId_SellableItemType",
                schema: "inventory_schema",
                table: "StockItems",
                columns: new[] { "SellableItemId", "SellableItemType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_IsConfirmed_IsReleased_ExpiresAt",
                schema: "inventory_schema",
                table: "StockReservations",
                columns: new[] { "IsConfirmed", "IsReleased", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_ReferenceId",
                schema: "inventory_schema",
                table: "StockReservations",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_StockItemId",
                schema: "inventory_schema",
                table: "StockReservations",
                column: "StockItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockReservations",
                schema: "inventory_schema");

            migrationBuilder.DropTable(
                name: "StockItems",
                schema: "inventory_schema");
        }
    }
}
