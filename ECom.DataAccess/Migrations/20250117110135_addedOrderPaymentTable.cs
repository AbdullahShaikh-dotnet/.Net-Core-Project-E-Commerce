using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedOrderPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Public_Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    razorpay_payment_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    razorpay_order_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    razorpay_signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPaymentSuccessfull = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderPayments_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_ApplicationUserID",
                table: "OrderPayments",
                column: "ApplicationUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPayments");
        }
    }
}
