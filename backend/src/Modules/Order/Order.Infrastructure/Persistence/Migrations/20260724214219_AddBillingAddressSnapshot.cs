using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingAddressSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine1",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine2",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingDistrict",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingPhoneNumber",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingPostalCode",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingRecipientName",
                schema: "order_schema",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddressLine1",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddressLine2",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingCountry",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingDistrict",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingPhoneNumber",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingPostalCode",
                schema: "order_schema",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingRecipientName",
                schema: "order_schema",
                table: "Orders");
        }
    }
}
