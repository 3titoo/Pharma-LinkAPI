using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharma_LinkAPI.Migrations
{
    /// <inheritdoc />
    public partial class aaqweqqwe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicineImage",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MedicineImage",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
