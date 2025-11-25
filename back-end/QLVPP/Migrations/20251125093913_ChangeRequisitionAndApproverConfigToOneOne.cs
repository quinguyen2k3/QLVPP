using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRequisitionAndApproverConfigToOneOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalStepApprovers");

            migrationBuilder.DropTable(
                name: "ApprovalStepInstances");

            migrationBuilder.DropTable(
                name: "ApprovalInstances");

            migrationBuilder.DropTable(
                name: "ApprovalSteps");

            migrationBuilder.CreateTable(
                name: "ApprovalConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiredApprovals = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalConfigs_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalProcesses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalProcesses_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Approvers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvers_ApprovalConfigs_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "ApprovalConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approvers_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    StepId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedToId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SequenceInGroup = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTasks_ApprovalConfigs_StepId",
                        column: x => x.StepId,
                        principalTable: "ApprovalConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTasks_ApprovalProcesses_ApprovalInstanceId",
                        column: x => x.ApprovalInstanceId,
                        principalTable: "ApprovalProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalTasks_Employee_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalTasks_Employee_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalConfigs_RequisitionId",
                table: "ApprovalConfigs",
                column: "RequisitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalProcesses_RequisitionId",
                table: "ApprovalProcesses",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_ApprovalInstanceId",
                table: "ApprovalTasks",
                column: "ApprovalInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_ApprovedById",
                table: "ApprovalTasks",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_AssignedToId",
                table: "ApprovalTasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_StepId",
                table: "ApprovalTasks",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvers_ConfigId",
                table: "Approvers",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvers_EmployeeId",
                table: "Approvers",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalTasks");

            migrationBuilder.DropTable(
                name: "Approvers");

            migrationBuilder.DropTable(
                name: "ApprovalProcesses");

            migrationBuilder.DropTable(
                name: "ApprovalConfigs");

            migrationBuilder.CreateTable(
                name: "ApprovalInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalInstances_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiredApprovals = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalSteps_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStepApprovers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    StepId = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalStepApprovers_ApprovalSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "ApprovalSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalStepApprovers_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
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
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    AssignedToId = table.Column<long>(type: "bigint", nullable: false),
                    StepId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApprovalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SequenceInGroup = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                        name: "FK_ApprovalStepInstances_ApprovalSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "ApprovalSteps",
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstances_RequisitionId",
                table: "ApprovalInstances",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepApprovers_EmployeeId",
                table: "ApprovalStepApprovers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepApprovers_StepId",
                table: "ApprovalStepApprovers",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_ApprovalInstanceId",
                table: "ApprovalStepInstances",
                column: "ApprovalInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_ApprovedById",
                table: "ApprovalStepInstances",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_AssignedToId",
                table: "ApprovalStepInstances",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepInstances_StepId",
                table: "ApprovalStepInstances",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_RequisitionId",
                table: "ApprovalSteps",
                column: "RequisitionId");
        }
    }
}
