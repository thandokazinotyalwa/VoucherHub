using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoucherHub.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherExpiryDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpiryDate",
                table: "Vouchers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

                
migrationBuilder.Sql(
    "UPDATE \"Vouchers\" SET \"ExpiryDate\" = CURRENT_DATE + 30");

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Vouchers");
        }
    }
}
