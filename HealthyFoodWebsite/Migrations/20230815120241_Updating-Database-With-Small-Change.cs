using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingDatabaseWithSmallChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Order",
                newName: "UserIsDeleted");

            migrationBuilder.AddColumn<bool>(
                name: "AdminIsDeleted",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminIsDeleted",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "UserIsDeleted",
                table: "Order",
                newName: "IsDeleted");
        }
    }
}
