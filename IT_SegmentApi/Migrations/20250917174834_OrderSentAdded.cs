using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_SegmentApi.Migrations
{
    /// <inheritdoc />
    public partial class OrderSentAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrderSent",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderSent",
                table: "Orders");
        }
    }
}
