using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoucherHub.Migrations
{
    /// <inheritdoc />
    public partial class StoreVoucherStatusAsText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "Vouchers"
        ALTER COLUMN "Status" TYPE text
        USING CASE "Status"
            WHEN 0 THEN 'Active'
            WHEN 1 THEN 'Redeemed'
            WHEN 2 THEN 'Expired'
            WHEN 3 THEN 'Cancelled'
            ELSE 'Active'
        END;
        """);
        }

        /// <inheritdoc />

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "Vouchers"
        ALTER COLUMN "Status" TYPE integer
        USING CASE "Status"
            WHEN 'Active' THEN 0
            WHEN 'Redeemed' THEN 1
            WHEN 'Expired' THEN 2
            WHEN 'Cancelled' THEN 3
            ELSE 0
        END;
        """);
        }
    }
}
