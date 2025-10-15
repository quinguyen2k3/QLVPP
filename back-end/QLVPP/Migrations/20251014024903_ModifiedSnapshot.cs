using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventorySnapshot_Products_ProductId",
                table: "InventorySnapshot");

            migrationBuilder.DropForeignKey(
                name: "FK_InventorySnapshot_Warehouse_WarehouseId",
                table: "InventorySnapshot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventorySnapshot",
                table: "InventorySnapshot");

            migrationBuilder.DropIndex(
                name: "IX_InventorySnapshot_ProductId",
                table: "InventorySnapshot");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InventorySnapshot");

            migrationBuilder.DropColumn(
                name: "TotalIssues",
                table: "InventorySnapshot");

            migrationBuilder.DropColumn(
                name: "TotalReceipts",
                table: "InventorySnapshot");

            migrationBuilder.RenameTable(
                name: "InventorySnapshot",
                newName: "InventorySnapshots");

            migrationBuilder.RenameIndex(
                name: "IX_InventorySnapshot_WarehouseId",
                table: "InventorySnapshots",
                newName: "IX_InventorySnapshots_WarehouseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventorySnapshots",
                table: "InventorySnapshots",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SnapshotDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    OpeningQty = table.Column<int>(type: "int", nullable: false),
                    TotalIn = table.Column<int>(type: "int", nullable: false),
                    TotalOut = table.Column<int>(type: "int", nullable: false),
                    ClosingQty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapshotDetail_InventorySnapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "InventorySnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnapshotDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotDetail_ProductId",
                table: "SnapshotDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotDetail_SnapshotId",
                table: "SnapshotDetail",
                column: "SnapshotId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventorySnapshots_Warehouse_WarehouseId",
                table: "InventorySnapshots",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventorySnapshots_Warehouse_WarehouseId",
                table: "InventorySnapshots");

            migrationBuilder.DropTable(
                name: "SnapshotDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventorySnapshots",
                table: "InventorySnapshots");

            migrationBuilder.RenameTable(
                name: "InventorySnapshots",
                newName: "InventorySnapshot");

            migrationBuilder.RenameIndex(
                name: "IX_InventorySnapshots_WarehouseId",
                table: "InventorySnapshot",
                newName: "IX_InventorySnapshot_WarehouseId");

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "InventorySnapshot",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "TotalIssues",
                table: "InventorySnapshot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalReceipts",
                table: "InventorySnapshot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventorySnapshot",
                table: "InventorySnapshot",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySnapshot_ProductId",
                table: "InventorySnapshot",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventorySnapshot_Products_ProductId",
                table: "InventorySnapshot",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventorySnapshot_Warehouse_WarehouseId",
                table: "InventorySnapshot",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
