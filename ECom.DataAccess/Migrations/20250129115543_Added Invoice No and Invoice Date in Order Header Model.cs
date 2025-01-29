using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedInvoiceNoandInvoiceDateinOrderHeaderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "OrderHeaders");
        }
    }
}
