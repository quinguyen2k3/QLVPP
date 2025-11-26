using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddDelagteIdForTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_StepId",
                table: "ApprovalTasks");

            migrationBuilder.RenameColumn(
                name: "StepId",
                table: "ApprovalTasks",
                newName: "ConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalTasks_StepId",
                table: "ApprovalTasks",
                newName: "IX_ApprovalTasks_ConfigId");

            migrationBuilder.AddColumn<long>(
                name: "DelegateId",
                table: "ApprovalTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_DelegateId",
                table: "ApprovalTasks",
                column: "DelegateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_ConfigId",
                table: "ApprovalTasks",
                column: "ConfigId",
                principalTable: "ApprovalConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTasks_Employee_DelegateId",
                table: "ApprovalTasks",
                column: "DelegateId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_ConfigId",
                table: "ApprovalTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTasks_Employee_DelegateId",
                table: "ApprovalTasks");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalTasks_DelegateId",
                table: "ApprovalTasks");

            migrationBuilder.DropColumn(
                name: "DelegateId",
                table: "ApprovalTasks");

            migrationBuilder.RenameColumn(
                name: "ConfigId",
                table: "ApprovalTasks",
                newName: "StepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalTasks_ConfigId",
                table: "ApprovalTasks",
                newName: "IX_ApprovalTasks_StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTasks_ApprovalConfigs_StepId",
                table: "ApprovalTasks",
                column: "StepId",
                principalTable: "ApprovalConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
