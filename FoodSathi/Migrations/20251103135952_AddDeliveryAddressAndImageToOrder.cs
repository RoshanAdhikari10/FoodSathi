using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSathi.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAddressAndImageToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderFeedbacks");

            migrationBuilder.AddColumn<string>(
                name: "ItemImage",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemImage",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "OrderFeedbacks",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFeedbacks", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_OrderFeedbacks_MenuItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "MenuItems",
                        principalColumn: "ItemID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderFeedbacks_ItemID",
                table: "OrderFeedbacks",
                column: "ItemID");
        }
    }
}
