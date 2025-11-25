using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class MergeApprovalTemplateIntoRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalInstances_ApprovalTemplates_TemplateId",
                table: "ApprovalInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalInstances_Department_RequesterDepartmentId",
                table: "ApprovalInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalInstances_Employee_RequesterId",
                table: "ApprovalInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepApprovers_ApprovalTemplateSteps_TemplateStepId",
                table: "ApprovalStepApprovers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_ApprovalTemplateSteps_TemplateStepId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Department_DepartmentId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTemplateSteps_ApprovalTemplates_TemplateId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropTable(
                name: "ApprovalTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalStepInstance_AssignedTo_Status",
                table: "ApprovalStepInstances");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalStepInstance_Status",
                table: "ApprovalStepInstances");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalStepInstances_DepartmentId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropIndex(
                name: "UX_ApprovalStepApprover_Step_Employee",
                table: "ApprovalStepApprovers");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalInstance_RequesterId",
                table: "ApprovalInstances");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalInstance_Status",
                table: "ApprovalInstances");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalInstances_RequesterDepartmentId",
                table: "ApprovalInstances");

            migrationBuilder.DropIndex(
                name: "UX_ApprovalInstance_Note",
                table: "ApprovalInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalTemplateSteps",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropIndex(
                name: "UX_ApprovalStep_Template_Order",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "ApprovalInstances");

            migrationBuilder.DropColumn(
                name: "NoteType",
                table: "ApprovalInstances");

            migrationBuilder.DropColumn(
                name: "RequesterDepartmentId",
                table: "ApprovalInstances");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "ApprovalInstances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "StepName",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "StepOrder",
                table: "ApprovalTemplateSteps");

            migrationBuilder.RenameTable(
                name: "ApprovalTemplateSteps",
                newName: "ApprovalSteps");

            migrationBuilder.RenameColumn(
                name: "TemplateStepId",
                table: "ApprovalStepInstances",
                newName: "StepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_TemplateStepId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_StepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstance_AssignedToId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_AssignedToId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstance_ApprovalInstanceId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_ApprovalInstanceId");

            migrationBuilder.RenameColumn(
                name: "TemplateStepId",
                table: "ApprovalStepApprovers",
                newName: "StepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepApprover_TemplateStepId",
                table: "ApprovalStepApprovers",
                newName: "IX_ApprovalStepApprovers_StepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepApprover_EmployeeId",
                table: "ApprovalStepApprovers",
                newName: "IX_ApprovalStepApprovers_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "ApprovalInstances",
                newName: "RequisitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalInstances_TemplateId",
                table: "ApprovalInstances",
                newName: "IX_ApprovalInstances_RequisitionId");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "ApprovalSteps",
                newName: "RequisitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStep_TemplateId",
                table: "ApprovalSteps",
                newName: "IX_ApprovalSteps_RequisitionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalSteps",
                table: "ApprovalSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalInstances_Requisitions_RequisitionId",
                table: "ApprovalInstances",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepApprovers_ApprovalSteps_StepId",
                table: "ApprovalStepApprovers",
                column: "StepId",
                principalTable: "ApprovalSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_ApprovalSteps_StepId",
                table: "ApprovalStepInstances",
                column: "StepId",
                principalTable: "ApprovalSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_Requisitions_RequisitionId",
                table: "ApprovalSteps",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalInstances_Requisitions_RequisitionId",
                table: "ApprovalInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepApprovers_ApprovalSteps_StepId",
                table: "ApprovalStepApprovers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_ApprovalSteps_StepId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_Requisitions_RequisitionId",
                table: "ApprovalSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalSteps",
                table: "ApprovalSteps");

            migrationBuilder.RenameTable(
                name: "ApprovalSteps",
                newName: "ApprovalTemplateSteps");

            migrationBuilder.RenameColumn(
                name: "StepId",
                table: "ApprovalStepInstances",
                newName: "TemplateStepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_StepId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_TemplateStepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_AssignedToId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstance_AssignedToId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_ApprovalInstanceId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstance_ApprovalInstanceId");

            migrationBuilder.RenameColumn(
                name: "StepId",
                table: "ApprovalStepApprovers",
                newName: "TemplateStepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepApprovers_StepId",
                table: "ApprovalStepApprovers",
                newName: "IX_ApprovalStepApprover_TemplateStepId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepApprovers_EmployeeId",
                table: "ApprovalStepApprovers",
                newName: "IX_ApprovalStepApprover_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "RequisitionId",
                table: "ApprovalInstances",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalInstances_RequisitionId",
                table: "ApprovalInstances",
                newName: "IX_ApprovalInstances_TemplateId");

            migrationBuilder.RenameColumn(
                name: "RequisitionId",
                table: "ApprovalTemplateSteps",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalSteps_RequisitionId",
                table: "ApprovalTemplateSteps",
                newName: "IX_ApprovalStep_TemplateId");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NoteId",
                table: "ApprovalInstances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "NoteType",
                table: "ApprovalInstances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "RequesterDepartmentId",
                table: "ApprovalInstances",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequesterId",
                table: "ApprovalInstances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ApprovalTemplateSteps",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StepName",
                table: "ApprovalTemplateSteps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepOrder",
                table: "ApprovalTemplateSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalTemplateSteps",
                table: "ApprovalTemplateSteps",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoteType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_AssignedTo_Status",
                table: "ApprovalStepInstances",
                columns: new[] { "AssignedToId", "Status" },
                filter: "[AssignedToId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstance_Status",
                table: "ApprovalStepInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_DepartmentId",
                table: "ApprovalStepInstances",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalStepApprover_Step_Employee",
                table: "ApprovalStepApprovers",
                columns: new[] { "TemplateStepId", "EmployeeId" },
                unique: true);

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
                name: "UX_ApprovalInstance_Note",
                table: "ApprovalInstances",
                columns: new[] { "NoteType", "NoteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalStep_Template_Order",
                table: "ApprovalTemplateSteps",
                columns: new[] { "TemplateId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplate_NoteType",
                table: "ApprovalTemplates",
                column: "NoteType");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalTemplate_Code",
                table: "ApprovalTemplates",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalInstances_ApprovalTemplates_TemplateId",
                table: "ApprovalInstances",
                column: "TemplateId",
                principalTable: "ApprovalTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalInstances_Department_RequesterDepartmentId",
                table: "ApprovalInstances",
                column: "RequesterDepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalInstances_Employee_RequesterId",
                table: "ApprovalInstances",
                column: "RequesterId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepApprovers_ApprovalTemplateSteps_TemplateStepId",
                table: "ApprovalStepApprovers",
                column: "TemplateStepId",
                principalTable: "ApprovalTemplateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_ApprovalTemplateSteps_TemplateStepId",
                table: "ApprovalStepInstances",
                column: "TemplateStepId",
                principalTable: "ApprovalTemplateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Department_DepartmentId",
                table: "ApprovalStepInstances",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTemplateSteps_ApprovalTemplates_TemplateId",
                table: "ApprovalTemplateSteps",
                column: "TemplateId",
                principalTable: "ApprovalTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
