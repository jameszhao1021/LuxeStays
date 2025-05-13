using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxeStays.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changePropertyNameForCheckInDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckoutDate",
                table: "Bookings",
                newName: "CheckOutDate");

            migrationBuilder.RenameColumn(
                name: "CheckinDate",
                table: "Bookings",
                newName: "CheckInDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckOutDate",
                table: "Bookings",
                newName: "CheckoutDate");

            migrationBuilder.RenameColumn(
                name: "CheckInDate",
                table: "Bookings",
                newName: "CheckinDate");
        }
    }
}
