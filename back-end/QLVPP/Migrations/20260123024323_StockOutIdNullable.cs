using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class StockOutIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_StockOut_DeliveryId",
                table: "Return");

            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "Return",
                newName: "StockOutId");

            migrationBuilder.RenameIndex(
                name: "IX_Return_DeliveryId",
                table: "Return",
                newName: "IX_Return_StockOutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return",
                column: "StockOutId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return");

            migrationBuilder.RenameColumn(
                name: "StockOutId",
                table: "Return",
                newName: "DeliveryId");

            migrationBuilder.RenameIndex(
                name: "IX_Return_StockOutId",
                table: "Return",
                newName: "IX_Return_DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_StockOut_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
