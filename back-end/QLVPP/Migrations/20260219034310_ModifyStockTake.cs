using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStockTake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "StockTakes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApproverId",
                table: "StockTakes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StockTakes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "StockTakes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "RequesterId",
                table: "StockTakes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RequesterId",
                table: "StockOut",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StockOut",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "StockOut",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceId",
                table: "StockIn",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StockIn",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_ApproverId",
                table: "StockTakes",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_RequesterId",
                table: "StockTakes",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakes_Employee_ApproverId",
                table: "StockTakes",
                column: "ApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakes_Employee_RequesterId",
                table: "StockTakes",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTakes_Employee_ApproverId",
                table: "StockTakes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakes_Employee_RequesterId",
                table: "StockTakes");

            migrationBuilder.DropIndex(
                name: "IX_StockTakes_ApproverId",
                table: "StockTakes");

            migrationBuilder.DropIndex(
                name: "IX_StockTakes_RequesterId",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "StockTakes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StockIn");

            migrationBuilder.AddColumn<long>(
                name: "PerformedById",
                table: "StockTakes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "RequesterId",
                table: "StockOut",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ReferenceId",
                table: "StockIn",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
