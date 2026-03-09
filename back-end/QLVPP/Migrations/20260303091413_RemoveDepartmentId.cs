using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialRequest_Department_DepartmentId",
                table: "MaterialRequest");

            migrationBuilder.DropIndex(
                name: "IX_MaterialRequest_DepartmentId",
                table: "MaterialRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequest_DepartmentId",
                table: "MaterialRequest",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialRequest_Department_DepartmentId",
                table: "MaterialRequest",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
