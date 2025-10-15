using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddFKProductReturnDeturnDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ReturnDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnDetail_ProductId",
                table: "ReturnDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnDetail_Products_ProductId",
                table: "ReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_ReturnDetail_ProductId",
                table: "ReturnDetail");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ReturnDetail");
        }
    }
}
