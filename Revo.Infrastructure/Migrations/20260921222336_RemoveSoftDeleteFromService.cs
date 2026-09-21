using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Revo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteFromService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
