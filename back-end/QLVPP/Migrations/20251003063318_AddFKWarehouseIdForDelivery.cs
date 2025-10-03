using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddFKWarehouseIdForDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "Delivery",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_WarehouseId",
                table: "Delivery",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Warehouse_WarehouseId",
                table: "Delivery",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Warehouse_WarehouseId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_WarehouseId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Delivery");
        }
    }
}
