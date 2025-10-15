using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedNotNullForReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ReturnDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DeliveryId",
                table: "Return",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ReturnDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "DeliveryId",
                table: "Return",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
