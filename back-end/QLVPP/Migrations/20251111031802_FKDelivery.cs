using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class FKDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "Transfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Delivery",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApproverId",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId1",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId2",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReceiverId",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequesterId",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ApproverId",
                table: "Delivery",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId",
                table: "Delivery",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId1",
                table: "Delivery",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId2",
                table: "Delivery",
                column: "EmployeeId2");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ReceiverId",
                table: "Delivery",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_RequesterId",
                table: "Delivery",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_ApproverId",
                table: "Delivery",
                column: "ApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId",
                table: "Delivery",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId1",
                table: "Delivery",
                column: "EmployeeId1",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId2",
                table: "Delivery",
                column: "EmployeeId2",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_ReceiverId",
                table: "Delivery",
                column: "ReceiverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_RequesterId",
                table: "Delivery",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_ApproverId",
                table: "Transfers",
                column: "ApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_ReceiverId",
                table: "Transfers",
                column: "ReceiverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Employee_RequesterId",
                table: "Transfers",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_ApproverId",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId2",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_ReceiverId",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_RequesterId",
                table: "Delivery");

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
                name: "IX_Delivery_ApproverId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId2",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_ReceiverId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_RequesterId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId2",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Delivery");

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
    }
}
