using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddApproverInstanceStepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Department_AssignedDepartmentId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalTemplate_NoteType_IsDefault",
                table: "ApprovalTemplates");

            migrationBuilder.DropIndex(
                name: "UX_ApprovalStepInstance_Instance_Order",
                table: "ApprovalStepInstances");

            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "ApprovalTemplateSteps",
                newName: "ApprovalType");

            migrationBuilder.RenameIndex(
                name: "UX_ApprovalTemplateStep_Template_Order",
                table: "ApprovalTemplateSteps",
                newName: "UX_ApprovalStep_Template_Order");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalTemplateStep_TemplateId",
                table: "ApprovalTemplateSteps",
                newName: "IX_ApprovalStep_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalTemplate_Code",
                table: "ApprovalTemplates",
                newName: "UX_ApprovalTemplate_Code");

            migrationBuilder.RenameColumn(
                name: "AssignedDepartmentId",
                table: "ApprovalStepInstances",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_AssignedDepartmentId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_DepartmentId");

            migrationBuilder.AlterColumn<long>(
                name: "PositionId",
                table: "ApprovalTemplateSteps",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ApprovalTemplateSteps",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredApprovals",
                table: "ApprovalTemplateSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PositionId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedToId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "ApprovalStepInstances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalType",
                table: "ApprovalStepInstances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "ApprovalStepInstances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SequenceInGroup",
                table: "ApprovalStepInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApprovalStepApprovers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateStepId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalStepApprovers_ApprovalTemplateSteps_TemplateStepId",
                        column: x => x.TemplateStepId,
                        principalTable: "ApprovalTemplateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalStepApprovers_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepApprover_EmployeeId",
                table: "ApprovalStepApprovers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepApprover_TemplateStepId",
                table: "ApprovalStepApprovers",
                column: "TemplateStepId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalStepApprover_Step_Employee",
                table: "ApprovalStepApprovers",
                columns: new[] { "TemplateStepId", "EmployeeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Department_DepartmentId",
                table: "ApprovalStepInstances",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Department_DepartmentId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropTable(
                name: "ApprovalStepApprovers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "RequiredApprovals",
                table: "ApprovalTemplateSteps");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "ApprovalStepInstances");

            migrationBuilder.DropColumn(
                name: "ApprovalType",
                table: "ApprovalStepInstances");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "ApprovalStepInstances");

            migrationBuilder.DropColumn(
                name: "SequenceInGroup",
                table: "ApprovalStepInstances");

            migrationBuilder.RenameColumn(
                name: "ApprovalType",
                table: "ApprovalTemplateSteps",
                newName: "Scope");

            migrationBuilder.RenameIndex(
                name: "UX_ApprovalStep_Template_Order",
                table: "ApprovalTemplateSteps",
                newName: "UX_ApprovalTemplateStep_Template_Order");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStep_TemplateId",
                table: "ApprovalTemplateSteps",
                newName: "IX_ApprovalTemplateStep_TemplateId");

            migrationBuilder.RenameIndex(
                name: "UX_ApprovalTemplate_Code",
                table: "ApprovalTemplates",
                newName: "IX_ApprovalTemplate_Code");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "ApprovalStepInstances",
                newName: "AssignedDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalStepInstances_DepartmentId",
                table: "ApprovalStepInstances",
                newName: "IX_ApprovalStepInstances_AssignedDepartmentId");

            migrationBuilder.AlterColumn<long>(
                name: "PositionId",
                table: "ApprovalTemplateSteps",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PositionId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedToId",
                table: "ApprovalStepInstances",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplate_NoteType_IsDefault",
                table: "ApprovalTemplates",
                columns: new[] { "NoteType", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalStepInstance_Instance_Order",
                table: "ApprovalStepInstances",
                columns: new[] { "ApprovalInstanceId", "StepOrder" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Department_AssignedDepartmentId",
                table: "ApprovalStepInstances",
                column: "AssignedDepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepInstances_Positions_PositionId",
                table: "ApprovalStepInstances",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalTemplateSteps_Positions_PositionId",
                table: "ApprovalTemplateSteps",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
