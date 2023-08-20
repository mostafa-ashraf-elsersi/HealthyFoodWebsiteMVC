using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingDatabaseWithAChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SeenByAdmin",
                table: "Testimonial",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeenByAdmin",
                table: "Testimonial");
        }
    }
}
