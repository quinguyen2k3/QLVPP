using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class MaterialRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialRequest",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialRequest_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequest_Employee_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequest_Employee_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequest_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ActorId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToUserId = table.Column<long>(type: "bigint", nullable: true),
                    Step = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaterialRequestId1 = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalLog_MaterialRequest_MaterialRequestId",
                        column: x => x.MaterialRequestId,
                        principalTable: "MaterialRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalLog_MaterialRequest_MaterialRequestId1",
                        column: x => x.MaterialRequestId1,
                        principalTable: "MaterialRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialRequestDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaterialRequestId1 = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequestDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId",
                        column: x => x.MaterialRequestId,
                        principalTable: "MaterialRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequestDetail_MaterialRequest_MaterialRequestId1",
                        column: x => x.MaterialRequestId1,
                        principalTable: "MaterialRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialRequestDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_MaterialRequestId",
                table: "ApprovalLog",
                column: "MaterialRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_MaterialRequestId1",
                table: "ApprovalLog",
                column: "MaterialRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequest_ApproverId",
                table: "MaterialRequest",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequest_DepartmentId",
                table: "MaterialRequest",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequest_RequesterId",
                table: "MaterialRequest",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequest_WarehouseId",
                table: "MaterialRequest",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequestDetail_MaterialRequestId",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequestDetail_MaterialRequestId1",
                table: "MaterialRequestDetail",
                column: "MaterialRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequestDetail_ProductId",
                table: "MaterialRequestDetail",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalLog");

            migrationBuilder.DropTable(
                name: "MaterialRequestDetail");

            migrationBuilder.DropTable(
                name: "MaterialRequest");
        }
    }
}
