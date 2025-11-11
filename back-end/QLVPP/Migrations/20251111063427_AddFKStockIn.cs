using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddFKStockIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "StockIn",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApproverId",
                table: "StockIn",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequesterId",
                table: "StockIn",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_ApproverId",
                table: "StockIn",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_RequesterId",
                table: "StockIn",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Employee_ApproverId",
                table: "StockIn",
                column: "ApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIn_Employee_RequesterId",
                table: "StockIn",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Employee_ApproverId",
                table: "StockIn");

            migrationBuilder.DropForeignKey(
                name: "FK_StockIn_Employee_RequesterId",
                table: "StockIn");

            migrationBuilder.DropIndex(
                name: "IX_StockIn_ApproverId",
                table: "StockIn");

            migrationBuilder.DropIndex(
                name: "IX_StockIn_RequesterId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "StockIn");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "StockIn");
        }
    }
}
