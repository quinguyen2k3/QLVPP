using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryToStockOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_Delivery_DeliveryId",
                table: "DeliveryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return");

            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.CreateTable(
                name: "StockOut",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint", nullable: true),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockOut_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockOut_Employee_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOut_Employee_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOut_Employee_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOut_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_ApproverId",
                table: "StockOut",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_DepartmentId",
                table: "StockOut",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_ReceiverId",
                table: "StockOut",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_RequesterId",
                table: "StockOut",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_WarehouseId",
                table: "StockOut",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_StockOut_DeliveryId",
                table: "DeliveryDetails",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_StockOut_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "StockOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_StockOut_DeliveryId",
                table: "DeliveryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Return_StockOut_DeliveryId",
                table: "Return");

            migrationBuilder.DropTable(
                name: "StockOut");

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: true),
                    RequesterId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delivery_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Delivery_Employee_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delivery_Employee_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delivery_Employee_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delivery_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ApproverId",
                table: "Delivery",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_DepartmentId",
                table: "Delivery",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ReceiverId",
                table: "Delivery",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_RequesterId",
                table: "Delivery",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_WarehouseId",
                table: "Delivery",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_Delivery_DeliveryId",
                table: "DeliveryDetails",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Delivery_DeliveryId",
                table: "Return",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
