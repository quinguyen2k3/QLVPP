using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStockIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Supplier_SupplierId",
                table: "StockIn");

            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Warehouse_WarehouseId",
                table: "StockIn");

            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                table: "StockIn",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "FromDepartmentId",
                table: "StockIn",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FromWarehouseId",
                table: "StockIn",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "StockIn",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReferenceId",
                table: "StockIn",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "StockIn",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_FromDepartmentId",
                table: "StockIn",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_FromWarehouseId",
                table: "StockIn",
                column: "FromWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Department_FromDepartmentId",
                table: "StockIn",
                column: "FromDepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Supplier_SupplierId",
                table: "StockIn",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Warehouse_FromWarehouseId",
                table: "StockIn",
                column: "FromWarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Warehouse_WarehouseId",
                table: "StockIn",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Department_FromDepartmentId",
                table: "StockIn");

            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Supplier_SupplierId",
                table: "StockIn");

            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Warehouse_FromWarehouseId",
                table: "StockIn");

            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Warehouse_WarehouseId",
                table: "StockIn");

            migrationBuilder.DropIndex(
                name: "IX_StockIn_FromDepartmentId",
                table: "StockIn");

            migrationBuilder.DropIndex(
                name: "IX_StockIn_FromWarehouseId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "FromDepartmentId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "FromWarehouseId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StockIn");

            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                table: "StockIn",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Supplier_SupplierId",
                table: "StockIn",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Warehouse_WarehouseId",
                table: "StockIn",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
