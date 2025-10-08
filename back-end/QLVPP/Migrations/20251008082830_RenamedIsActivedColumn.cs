using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class RenamedIsActivedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Warehouse",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Unit",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Supplier",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Return",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Requisitions",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Products",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Order",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Employee",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Department",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Delivery",
                newName: "IsActivated");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Category",
                newName: "IsActivated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Warehouse",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Unit",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Supplier",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Return",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Requisitions",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Products",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Order",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Employee",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Department",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Delivery",
                newName: "IsActived");

            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Category",
                newName: "IsActived");
        }
    }
}
