using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddePerformenceById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PerformedById",
                table: "StockTakes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_PerformedById",
                table: "StockTakes",
                column: "PerformedById");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakes_Employee_PerformedById",
                table: "StockTakes",
                column: "PerformedById",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTakes_Employee_PerformedById",
                table: "StockTakes");

            migrationBuilder.DropIndex(
                name: "IX_StockTakes_PerformedById",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "PerformedById",
                table: "StockTakes");
        }
    }
}
