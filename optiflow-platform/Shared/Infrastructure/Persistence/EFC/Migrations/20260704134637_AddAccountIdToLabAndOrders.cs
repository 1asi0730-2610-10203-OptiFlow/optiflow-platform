using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToLabAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_laboratories_name",
                table: "laboratories");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "work_orders",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "laboratories",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_work_orders_account_id",
                table: "work_orders",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_laboratories_account_id_name",
                table: "laboratories",
                columns: new[] { "account_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_work_orders_account_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "i_x_laboratories_account_id_name",
                table: "laboratories");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "laboratories");

            migrationBuilder.CreateIndex(
                name: "i_x_laboratories_name",
                table: "laboratories",
                column: "name",
                unique: true);
        }
    }
}
