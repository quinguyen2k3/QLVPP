using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseIdForOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Received",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "Order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Order_WarehouseId",
                table: "Order",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Warehouse_WarehouseId",
                table: "Order",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Warehouse_WarehouseId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_WarehouseId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Received",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Order");
        }
    }
}
