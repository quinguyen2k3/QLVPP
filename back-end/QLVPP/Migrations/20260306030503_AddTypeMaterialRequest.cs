using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeMaterialRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "MaterialRequest");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "MaterialRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MaterialRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "MaterialRequest");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MaterialRequest");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "MaterialRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
