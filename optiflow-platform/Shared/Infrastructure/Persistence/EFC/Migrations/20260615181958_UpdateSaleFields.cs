using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discount_percentage",
                table: "sales");

            migrationBuilder.RenameColumn(
                name: "sale_date",
                table: "sales",
                newName: "payment_method");

            migrationBuilder.RenameColumn(
                name: "quota_amount",
                table: "sales",
                newName: "pending_balance");

            migrationBuilder.RenameColumn(
                name: "client_name",
                table: "sales",
                newName: "user_name");

            migrationBuilder.AddColumn<decimal>(
                name: "adelanto",
                table: "sales",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "created_at",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "delivered_at",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                table: "sales",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "discount_code",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "invoice_number",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "lab_order_number",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "sales",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "patient_id",
                table: "sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "patient_name",
                table: "sales",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "patient_rx",
                table: "sales",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "sale_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sale_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sale_items_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_sale_items_sale_id",
                table: "sale_items",
                column: "sale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_items");

            migrationBuilder.DropColumn(
                name: "adelanto",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "delivered_at",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "discount_code",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "invoice_number",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "lab_order_number",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "patient_id",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "patient_name",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "patient_rx",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "sales");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "sales",
                newName: "client_name");

            migrationBuilder.RenameColumn(
                name: "pending_balance",
                table: "sales",
                newName: "quota_amount");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "sales",
                newName: "sale_date");

            migrationBuilder.AddColumn<decimal>(
                name: "discount_percentage",
                table: "sales",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
