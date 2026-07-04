using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_suppliers_name",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "i_x_products_sku",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "suppliers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "stock_audit_logs",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "products",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_suppliers_account_id_name",
                table: "suppliers",
                columns: new[] { "account_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_stock_audit_logs_account_id",
                table: "stock_audit_logs",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_products_account_id_sku",
                table: "products",
                columns: new[] { "account_id", "sku" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_suppliers_account_id_name",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "i_x_stock_audit_logs_account_id",
                table: "stock_audit_logs");

            migrationBuilder.DropIndex(
                name: "i_x_products_account_id_sku",
                table: "products");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "stock_audit_logs");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "products");

            migrationBuilder.CreateIndex(
                name: "i_x_suppliers_name",
                table: "suppliers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_products_sku",
                table: "products",
                column: "sku",
                unique: true);
        }
    }
}
