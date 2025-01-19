using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedPaymentSignaturecolumnforRazorPaySignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentSignature",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentSignature",
                table: "OrderHeaders");
        }
    }
}
