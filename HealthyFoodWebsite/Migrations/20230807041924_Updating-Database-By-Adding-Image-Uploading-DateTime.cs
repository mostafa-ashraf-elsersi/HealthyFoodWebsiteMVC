using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingDatabaseByAddingImageUploadingDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UploadingDateAndTime",
                table: "Product",
                type: "datetimeoffset",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadingDateAndTime",
                table: "Product");
        }
    }
}
