using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class SetWarehouseIdToNotNullOnDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
            name: "WarehouseId",
            table: "Delivery",
            type: "bigint",
            nullable: false, // Đây là thay đổi quan trọng
            defaultValue: 0L, // Có thể có hoặc không tùy phiên bản EF
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
