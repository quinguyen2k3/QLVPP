using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class AddIdForDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RequisitionDetails",
                table: "RequisitionDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "RequisitionDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "OrderDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "DeliveryDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequisitionDetails",
                table: "RequisitionDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionDetails_RequisitionId",
                table: "RequisitionDetails",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_DeliveryId",
                table: "DeliveryDetails",
                column: "DeliveryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RequisitionDetails",
                table: "RequisitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_RequisitionDetails_RequisitionId",
                table: "RequisitionDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDetails_DeliveryId",
                table: "DeliveryDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RequisitionDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DeliveryDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequisitionDetails",
                table: "RequisitionDetails",
                columns: new[] { "RequisitionId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail",
                columns: new[] { "OrderId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryDetails",
                table: "DeliveryDetails",
                columns: new[] { "DeliveryId", "ProductId" });
        }
    }
}
