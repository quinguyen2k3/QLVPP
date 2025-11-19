using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApproveColumnsFromRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Employee_CurrentApproverId",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Employee_OriginalApproverId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_CurrentApproverId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_OriginalApproverId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "CurrentApproverId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "OriginalApproverId",
                table: "Requisitions");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Requisitions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "Requisitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Requisitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_DepartmentId",
                table: "Requisitions",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Department_DepartmentId",
                table: "Requisitions",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Department_DepartmentId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_DepartmentId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Requisitions");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Requisitions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Requisitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrentApproverId",
                table: "Requisitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OriginalApproverId",
                table: "Requisitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_CurrentApproverId",
                table: "Requisitions",
                column: "CurrentApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_OriginalApproverId",
                table: "Requisitions",
                column: "OriginalApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Employee_CurrentApproverId",
                table: "Requisitions",
                column: "CurrentApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Employee_OriginalApproverId",
                table: "Requisitions",
                column: "OriginalApproverId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
