using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class StockOutId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockOutDetail_StockOut_DeliveryId",
                table: "StockOutDetail");

            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "StockOutDetail",
                newName: "StockOutId");

            migrationBuilder.RenameIndex(
                name: "IX_StockOutDetail_DeliveryId",
                table: "StockOutDetail",
                newName: "IX_StockOutDetail_StockOutId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockOutDetail_StockOut_StockOutId",
                table: "StockOutDetail",
                column: "StockOutId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockOutDetail_StockOut_StockOutId",
                table: "StockOutDetail");

            migrationBuilder.RenameColumn(
                name: "StockOutId",
                table: "StockOutDetail",
                newName: "DeliveryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockOutDetail_StockOutId",
                table: "StockOutDetail",
                newName: "IX_StockOutDetail_DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockOutDetail_StockOut_DeliveryId",
                table: "StockOutDetail",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
