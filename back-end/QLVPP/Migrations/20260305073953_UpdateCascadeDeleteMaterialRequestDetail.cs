using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteMaterialRequestDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId",
                table: "MaterialRequestDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId",
                principalTable: "MaterialRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId",
                table: "MaterialRequestDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId",
                principalTable: "MaterialRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
