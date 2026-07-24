using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressTypeFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBillingAddress",
                schema: "customer_schema",
                table: "CustomerAddresses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShippingAddress",
                schema: "customer_schema",
                table: "CustomerAddresses",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBillingAddress",
                schema: "customer_schema",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "IsShippingAddress",
                schema: "customer_schema",
                table: "CustomerAddresses");
        }
    }
}
