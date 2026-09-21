using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Revo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteFromPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PortfolioMedia");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PortfolioItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PortfolioMedia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PortfolioItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
