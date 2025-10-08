using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class FKEmployeeWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "Employee",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_WarehouseId",
                table: "Employee",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Warehouse_WarehouseId",
                table: "Employee",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Warehouse_WarehouseId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_WarehouseId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Employee");
        }
    }
}
