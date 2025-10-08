using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedReturnDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_Return_ReturnId",
                table: "ReturnDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                principalTable: "DeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_Return_ReturnId",
                table: "ReturnDetail",
                column: "ReturnId",
                principalTable: "Return",
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
                name: "FK_ReturnDetail_Return_ReturnId",
                table: "ReturnDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                principalTable: "DeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_Return_ReturnId",
                table: "ReturnDetail",
                column: "ReturnId",
                principalTable: "Return",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
