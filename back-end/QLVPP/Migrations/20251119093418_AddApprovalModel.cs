using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStep_Employee_AssignedToId",
                table: "ApprovalStep");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStep_Requisitions_RequisitionId",
                table: "ApprovalStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalStep",
                table: "ApprovalStep");

            migrationBuilder.RenameTable(
                name: "ApprovalStep",
                newName: "ApprovalSteps");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStep_RequisitionId",
                table: "ApprovalSteps",
                newName: "IX_ApprovalSteps_RequisitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStep_AssignedToId",
                table: "ApprovalSteps",
                newName: "IX_ApprovalSteps_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalSteps",
                table: "ApprovalSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_Employee_AssignedToId",
                table: "ApprovalSteps",
                column: "AssignedToId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_Requisitions_RequisitionId",
                table: "ApprovalSteps",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_Employee_AssignedToId",
                table: "ApprovalSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_Requisitions_RequisitionId",
                table: "ApprovalSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalSteps",
                table: "ApprovalSteps");

            migrationBuilder.RenameTable(
                name: "ApprovalSteps",
                newName: "ApprovalStep");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalSteps_RequisitionId",
                table: "ApprovalStep",
                newName: "IX_ApprovalStep_RequisitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalSteps_AssignedToId",
                table: "ApprovalStep",
                newName: "IX_ApprovalStep_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalStep",
                table: "ApprovalStep",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStep_Employee_AssignedToId",
                table: "ApprovalStep",
                column: "AssignedToId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStep_Requisitions_RequisitionId",
                table: "ApprovalStep",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
