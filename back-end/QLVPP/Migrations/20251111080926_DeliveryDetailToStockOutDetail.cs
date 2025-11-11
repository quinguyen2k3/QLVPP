using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryDetailToStockOutDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_Products_ProductId",
                table: "DeliveryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_StockOut_DeliveryId",
                table: "DeliveryDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails");

            migrationBuilder.RenameTable(
                name: "DeliveryDetails",
                newName: "StockOutDetail");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryDetails_ProductId",
                table: "StockOutDetail",
                newName: "IX_StockOutDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryDetails_DeliveryId",
                table: "StockOutDetail",
                newName: "IX_StockOutDetail_DeliveryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockOutDetail",
                table: "StockOutDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockOutDetail_Products_ProductId",
                table: "StockOutDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOutDetail_StockOut_DeliveryId",
                table: "StockOutDetail",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockOutDetail_Products_ProductId",
                table: "StockOutDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOutDetail_StockOut_DeliveryId",
                table: "StockOutDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockOutDetail",
                table: "StockOutDetail");

            migrationBuilder.RenameTable(
                name: "StockOutDetail",
                newName: "DeliveryDetails");

            migrationBuilder.RenameIndex(
                name: "IX_StockOutDetail_ProductId",
                table: "DeliveryDetails",
                newName: "IX_DeliveryDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StockOutDetail_DeliveryId",
                table: "DeliveryDetails",
                newName: "IX_DeliveryDetails_DeliveryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_Products_ProductId",
                table: "DeliveryDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_StockOut_DeliveryId",
                table: "DeliveryDetails",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
