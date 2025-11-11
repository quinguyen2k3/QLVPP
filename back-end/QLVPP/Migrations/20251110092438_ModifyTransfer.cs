using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApproverId",
                table: "Transfers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReceiverId",
                table: "Transfers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequesterId",
                table: "Transfers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ApproverId",
                table: "Transfers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_RequesterId",
                table: "Transfers",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_ApproverId",
                table: "Transfers",
                column: "ApproverId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_ReceiverId",
                table: "Transfers",
                column: "ReceiverId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_RequesterId",
                table: "Transfers",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Employee_ApproverId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Employee_ReceiverId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Employee_RequesterId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_ApproverId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_RequesterId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Transfers");
        }
    }
}
