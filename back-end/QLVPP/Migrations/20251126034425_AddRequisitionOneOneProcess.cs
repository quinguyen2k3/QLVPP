using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddRequisitionOneOneProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses",
                column: "RequisitionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses",
                column: "RequisitionId");
        }
    }
}
