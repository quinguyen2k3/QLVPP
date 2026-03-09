using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SnapshotDate",
                table: "InventorySnapshots",
                newName: "ToDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDate",
                table: "InventorySnapshots",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "InventorySnapshots");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "InventorySnapshots",
                newName: "SnapshotDate");
        }
    }
}
