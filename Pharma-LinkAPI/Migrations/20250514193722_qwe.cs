using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharma_LinkAPI.Migrations
{
    /// <inheritdoc />
    public partial class qwe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutUs",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutUs",
                table: "AspNetUsers");
        }
    }
}
