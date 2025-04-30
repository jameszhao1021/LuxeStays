using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxeStays.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifyVillaNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Villa_number",
                table: "VillaNumbers",
                newName: "Villa_Number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Villa_Number",
                table: "VillaNumbers",
                newName: "Villa_number");
        }
    }
}
