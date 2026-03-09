using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class FixApprovalLogRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalLog_MaterialRequest_MaterialRequestId1",
                table: "ApprovalLog");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId1",
                table: "MaterialRequestDetail");

            migrationBuilder.DropIndex(
                name: "IX_MaterialRequestDetail_MaterialRequestId1",
                table: "MaterialRequestDetail");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalLog_MaterialRequestId1",
                table: "ApprovalLog");

            migrationBuilder.DropColumn(
                name: "MaterialRequestId1",
                table: "MaterialRequestDetail");

            migrationBuilder.DropColumn(
                name: "MaterialRequestId1",
                table: "ApprovalLog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MaterialRequestId1",
                table: "MaterialRequestDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaterialRequestId1",
                table: "ApprovalLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequestDetail_MaterialRequestId1",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_MaterialRequestId1",
                table: "ApprovalLog",
                column: "MaterialRequestId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLog_MaterialRequest_MaterialRequestId1",
                table: "ApprovalLog",
                column: "MaterialRequestId1",
                principalTable: "MaterialRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId1",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId1",
                principalTable: "MaterialRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
