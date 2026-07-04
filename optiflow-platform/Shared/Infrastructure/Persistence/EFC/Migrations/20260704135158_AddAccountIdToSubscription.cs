using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "subscriptions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "subscription_payments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "subscription_billings",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_account_id",
                table: "subscriptions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_payments_account_id",
                table: "subscription_payments",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_billings_account_id",
                table: "subscription_billings",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_subscriptions_account_id",
                table: "subscriptions");

            migrationBuilder.DropIndex(
                name: "i_x_subscription_payments_account_id",
                table: "subscription_payments");

            migrationBuilder.DropIndex(
                name: "i_x_subscription_billings_account_id",
                table: "subscription_billings");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "subscription_payments");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "subscription_billings");
        }
    }
}
