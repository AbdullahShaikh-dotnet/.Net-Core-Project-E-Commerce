using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedisOrderPlacedColumninShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isOrderPlaced",
                table: "ShoppingCarts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isOrderPlaced",
                table: "ShoppingCarts");
        }
    }
}
