using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAprovalProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTasks_ApprovalProcesses_ApprovalInstanceId",
                table: "ApprovalTasks");

            migrationBuilder.DropTable(
                name: "ApprovalProcesses");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalTasks_ApprovalInstanceId",
                table: "ApprovalTasks");

            migrationBuilder.DropColumn(
                name: "ApprovalInstanceId",
                table: "ApprovalTasks");

            migrationBuilder.AddColumn<long>(
                name: "ApprovalConfigId",
                table: "ApprovalTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_ApprovalConfigId",
                table: "ApprovalTasks",
                column: "ApprovalConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_ApprovalConfigId",
                table: "ApprovalTasks",
                column: "ApprovalConfigId",
                principalTable: "ApprovalConfigs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_ApprovalConfigId",
                table: "ApprovalTasks");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalTasks_ApprovalConfigId",
                table: "ApprovalTasks");

            migrationBuilder.DropColumn(
                name: "ApprovalConfigId",
                table: "ApprovalTasks");

            migrationBuilder.AddColumn<long>(
                name: "ApprovalInstanceId",
                table: "ApprovalTasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ApprovalProcesses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalProcesses_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_ApprovalInstanceId",
                table: "ApprovalTasks",
                column: "ApprovalInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses",
                column: "RequisitionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTasks_ApprovalProcesses_ApprovalInstanceId",
                table: "ApprovalTasks",
                column: "ApprovalInstanceId",
                principalTable: "ApprovalProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
