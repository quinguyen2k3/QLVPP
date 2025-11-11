using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFKDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Employee_EmployeeId2",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_EmployeeId2",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "EmployeeId2",
                table: "Delivery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId1",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId2",
                table: "Delivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId",
                table: "Delivery",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId1",
                table: "Delivery",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_EmployeeId2",
                table: "Delivery",
                column: "EmployeeId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId",
                table: "Delivery",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId1",
                table: "Delivery",
                column: "EmployeeId1",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Employee_EmployeeId2",
                table: "Delivery",
                column: "EmployeeId2",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
