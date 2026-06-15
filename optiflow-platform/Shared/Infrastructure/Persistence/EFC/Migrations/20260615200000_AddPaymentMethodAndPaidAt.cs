using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodAndPaidAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE `payments`
                    ADD COLUMN IF NOT EXISTS `method` varchar(50) NOT NULL DEFAULT '',
                    ADD COLUMN IF NOT EXISTS `paid_at` varchar(50) NOT NULL DEFAULT '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "method", table: "payments");
            migrationBuilder.DropColumn(name: "paid_at", table: "payments");
        }
    }
}
