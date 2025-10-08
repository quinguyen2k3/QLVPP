using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAssetLoanKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "AssetLoan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail",
                column: "AssetLoanId",
                principalTable: "AssetLoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan");

            migrationBuilder.DropIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssetLoan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan",
                column: "DeliveryDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_AssetLoan_AssetLoanId",
                table: "ReturnDetail",
                column: "AssetLoanId",
                principalTable: "AssetLoan",
                principalColumn: "DeliveryDetailId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
