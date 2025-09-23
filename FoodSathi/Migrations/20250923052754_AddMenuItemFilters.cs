using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSathi.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FoodType",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpicyLevel",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodType",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "SpicyLevel",
                table: "MenuItems");
        }
    }
}
