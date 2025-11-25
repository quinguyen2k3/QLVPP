using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DeletePositionIdInApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalTemplateSteps_PositionId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalStepInstances_PositionId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "ApprovalStepInstances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PositionId",
                table: "ApprovalTemplateSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PositionId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateSteps_PositionId",
                table: "ApprovalTemplateSteps",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_PositionId",
                table: "ApprovalStepInstances",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }
    }
}
