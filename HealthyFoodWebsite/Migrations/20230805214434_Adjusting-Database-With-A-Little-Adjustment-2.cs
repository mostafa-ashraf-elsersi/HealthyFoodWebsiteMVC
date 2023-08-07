using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyFoodWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AdjustingDatabaseWithALittleAdjustment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublishingDate",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishingDate",
                table: "Blog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
