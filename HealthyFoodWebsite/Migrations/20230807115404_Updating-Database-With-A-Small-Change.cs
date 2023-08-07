using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingDatabaseWithASmallChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ShoppingBag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShoppingBag");
        }
    }
}
