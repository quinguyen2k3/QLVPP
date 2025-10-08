using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAssetLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan");

            migrationBuilder.DropIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "IsActived",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "AssetLoan");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AssetLoan",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan",
                column: "DeliveryDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AssetLoan");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "AssetLoan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AssetLoan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetLoan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "AssetLoan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActived",
                table: "AssetLoan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "AssetLoan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "AssetLoan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "AssetLoan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "AssetLoan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetLoan",
                table: "AssetLoan",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId",
                unique: true);
        }
    }
}
