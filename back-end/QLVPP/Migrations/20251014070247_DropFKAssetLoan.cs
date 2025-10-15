using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DropFKAssetLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotDetail_InventorySnapshots_SnapshotId",
                table: "SnapshotDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotDetail_Products_ProductId",
                table: "SnapshotDetail");

            migrationBuilder.DropIndex(
                name: "IX_ReturnDetail_AssetLoanId",
                table: "ReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SnapshotDetail",
                table: "SnapshotDetail");

            migrationBuilder.DropColumn(
                name: "AssetLoanId",
                table: "ReturnDetail");

            migrationBuilder.RenameTable(
                name: "SnapshotDetail",
                newName: "SnapshotDetails");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotDetail_SnapshotId",
                table: "SnapshotDetails",
                newName: "IX_SnapshotDetails_SnapshotId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotDetail_ProductId",
                table: "SnapshotDetails",
                newName: "IX_SnapshotDetails_ProductId");

            migrationBuilder.AddColumn<long>(
                name: "DeliveryId",
                table: "Return",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SnapshotDetails",
                table: "SnapshotDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Return_DeliveryId",
                table: "Return",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                principalTable: "DeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotDetails_InventorySnapshots_SnapshotId",
                table: "SnapshotDetails",
                column: "SnapshotId",
                principalTable: "InventorySnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotDetails_Products_ProductId",
                table: "SnapshotDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotDetails_InventorySnapshots_SnapshotId",
                table: "SnapshotDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotDetails_Products_ProductId",
                table: "SnapshotDetails");

            migrationBuilder.DropIndex(
                name: "IX_Return_DeliveryId",
                table: "Return");

            migrationBuilder.DropIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SnapshotDetails",
                table: "SnapshotDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Return");

            migrationBuilder.RenameTable(
                name: "SnapshotDetails",
                newName: "SnapshotDetail");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotDetails_SnapshotId",
                table: "SnapshotDetail",
                newName: "IX_SnapshotDetail_SnapshotId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotDetails_ProductId",
                table: "SnapshotDetail",
                newName: "IX_SnapshotDetail_ProductId");

            migrationBuilder.AddColumn<long>(
                name: "AssetLoanId",
                table: "ReturnDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SnapshotDetail",
                table: "SnapshotDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnDetail_AssetLoanId",
                table: "ReturnDetail",
                column: "AssetLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                principalTable: "DeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail",
                column: "AssetLoanId",
                principalTable: "AssetLoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotDetail_InventorySnapshots_SnapshotId",
                table: "SnapshotDetail",
                column: "SnapshotId",
                principalTable: "InventorySnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotDetail_Products_ProductId",
                table: "SnapshotDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
