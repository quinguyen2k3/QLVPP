using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStockOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOut_Department_DepartmentId",
                table: "StockOut");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOut_Warehouse_WarehouseId",
                table: "StockOut");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "StockOut",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "StockOut",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ToWarehouseId",
                table: "StockOut",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "StockOut",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_ToWarehouseId",
                table: "StockOut",
                column: "ToWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return",
                column: "StockOutId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOut_Department_DepartmentId",
                table: "StockOut",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOut_Warehouse_ToWarehouseId",
                table: "StockOut",
                column: "ToWarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOut_Warehouse_WarehouseId",
                table: "StockOut",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOut_Department_DepartmentId",
                table: "StockOut");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOut_Warehouse_ToWarehouseId",
                table: "StockOut");

            migrationBuilder.DropForeignKey(
                name: "FK_StockOut_Warehouse_WarehouseId",
                table: "StockOut");

            migrationBuilder.DropIndex(
                name: "IX_StockOut_ToWarehouseId",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "ToWarehouseId",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StockOut");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "StockOut",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_StockOut_StockOutId",
                table: "Return",
                column: "StockOutId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOut_Department_DepartmentId",
                table: "StockOut",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockOut_Warehouse_WarehouseId",
                table: "StockOut",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
