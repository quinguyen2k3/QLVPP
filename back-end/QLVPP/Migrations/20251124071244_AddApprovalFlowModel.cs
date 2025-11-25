using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFlowModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Department_DepartmentId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Position_PositionId",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Position",
                table: "Position");

            migrationBuilder.RenameTable(
                name: "Position",
                newName: "Positions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoteType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<long>(type: "bigint", nullable: false),
                    NoteType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoteId = table.Column<long>(type: "bigint", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint", nullable: false),
                    RequesterDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalInstances_ApprovalTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalInstances_Department_RequesterDepartmentId",
                        column: x => x.RequesterDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalInstances_Employee_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTemplateSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<long>(type: "bigint", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplateSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateSteps_ApprovalTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStepInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateStepId = table.Column<long>(type: "bigint", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedToId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_ApprovalInstances_ApprovalInstanceId",
                        column: x => x.ApprovalInstanceId,
                        principalTable: "ApprovalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_ApprovalTemplateSteps_TemplateStepId",
                        column: x => x.TemplateStepId,
                        principalTable: "ApprovalTemplateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_Department_AssignedDepartmentId",
                        column: x => x.AssignedDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_Employee_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_Employee_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepInstances_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstance_RequesterId",
                table: "ApprovalInstances",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstance_Status",
                table: "ApprovalInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstances_RequesterDepartmentId",
                table: "ApprovalInstances",
                column: "RequesterDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstances_TemplateId",
                table: "ApprovalInstances",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalInstance_Note",
                table: "ApprovalInstances",
                columns: new[] { "NoteType", "NoteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_ApprovalInstanceId",
                table: "ApprovalStepInstances",
                column: "ApprovalInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_AssignedTo_Status",
                table: "ApprovalStepInstances",
                columns: new[] { "AssignedToId", "Status" },
                filter: "[AssignedToId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_AssignedToId",
                table: "ApprovalStepInstances",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_Status",
                table: "ApprovalStepInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_ApprovedById",
                table: "ApprovalStepInstances",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_AssignedDepartmentId",
                table: "ApprovalStepInstances",
                column: "AssignedDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_PositionId",
                table: "ApprovalStepInstances",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_TemplateStepId",
                table: "ApprovalStepInstances",
                column: "TemplateStepId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalStepInstance_Instance_Order",
                table: "ApprovalStepInstances",
                columns: new[] { "ApprovalInstanceId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplate_Code",
                table: "ApprovalTemplates",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplate_NoteType",
                table: "ApprovalTemplates",
                column: "NoteType");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplate_NoteType_IsDefault",
                table: "ApprovalTemplates",
                columns: new[] { "NoteType", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateStep_TemplateId",
                table: "ApprovalTemplateSteps",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateSteps_PositionId",
                table: "ApprovalTemplateSteps",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalTemplateStep_Template_Order",
                table: "ApprovalTemplateSteps",
                columns: new[] { "TemplateId", "StepOrder" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Department_DepartmentId",
                table: "Employee",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Positions_PositionId",
                table: "Employee",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Department_DepartmentId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Positions_PositionId",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "ApprovalStepInstances");

            migrationBuilder.DropTable(
                name: "ApprovalInstances");

            migrationBuilder.DropTable(
                name: "ApprovalTemplateSteps");

            migrationBuilder.DropTable(
                name: "ApprovalTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "Position");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Position",
                table: "Position",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Department_DepartmentId",
                table: "Employee",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Position_PositionId",
                table: "Employee",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id");
        }
    }
}
