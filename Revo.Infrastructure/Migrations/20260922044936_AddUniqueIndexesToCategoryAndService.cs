using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Revo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesToCategoryAndService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Services_NameAr",
                table: "Services",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_NameEn",
                table: "Services",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameAr",
                table: "Categories",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_NameAr",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_NameEn",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameAr",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameEn",
                table: "Categories");
        }
    }
}
