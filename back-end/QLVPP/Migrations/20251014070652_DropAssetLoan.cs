using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLVPP.Migrations
{
    /// <inheritdoc />
    public partial class DropAssetLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetLoan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetLoan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryDetailId = table.Column<long>(type: "bigint", nullable: false),
                    DamagedQuantity = table.Column<int>(type: "int", nullable: false),
                    IssuedQuantity = table.Column<int>(type: "int", nullable: false),
                    ReturnedQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLoan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetLoan_DeliveryDetails_DeliveryDetailId",
                        column: x => x.DeliveryDetailId,
                        principalTable: "DeliveryDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetLoan_DeliveryDetailId",
                table: "AssetLoan",
                column: "DeliveryDetailId");
        }
    }
}
