using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxeStays.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changePropertyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChekinDate",
                table: "Bookings",
                newName: "CheckinDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckinDate",
                table: "Bookings",
                newName: "ChekinDate");
        }
    }
}
