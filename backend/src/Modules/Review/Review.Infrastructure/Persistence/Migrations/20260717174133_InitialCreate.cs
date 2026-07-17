using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Review.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "review_schema");

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "review_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellableItemType = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SellableItemId_SellableItemType",
                schema: "review_schema",
                table: "Reviews",
                columns: new[] { "SellableItemId", "SellableItemType" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_SellableItemId_SellableItemType",
                schema: "review_schema",
                table: "Reviews",
                columns: new[] { "UserId", "SellableItemId", "SellableItemType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "review_schema");
        }
    }
}
