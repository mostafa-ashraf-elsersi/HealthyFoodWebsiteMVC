using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AdjustingDatabaseWithALittleAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Blog",
                newName: "AuthorType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorType",
                table: "Blog",
                newName: "Author");
        }
    }
}
