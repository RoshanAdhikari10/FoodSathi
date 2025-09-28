using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSathi.Migrations
{
    /// <inheritdoc />
    public partial class FixOfferTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemID",
                table: "Offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemID",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
