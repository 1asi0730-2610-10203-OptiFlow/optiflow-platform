using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToSalesAndSystemNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "system_notifications",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "sales",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "sale_items",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "payments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_system_notifications_account_id",
                table: "system_notifications",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sales_account_id",
                table: "sales",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sale_items_account_id",
                table: "sale_items",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_payments_account_id",
                table: "payments",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_system_notifications_account_id",
                table: "system_notifications");

            migrationBuilder.DropIndex(
                name: "i_x_sales_account_id",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "i_x_sale_items_account_id",
                table: "sale_items");

            migrationBuilder.DropIndex(
                name: "i_x_payments_account_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "system_notifications");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "payments");
        }
    }
}
