using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddIamTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "prescription_uuid",
                table: "prescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "customer_uuid",
                table: "patients",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "birth_date",
                table: "patients",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "clinical_record_uuid",
                table: "clinical_records",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateTable(
                name: "password_recovery_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_password_recovery_tokens", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "longtext", nullable: false),
                    google_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_prescriptions_prescription_uuid",
                table: "prescriptions",
                column: "prescription_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_patients_customer_uuid",
                table: "patients",
                column: "customer_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_clinical_records_clinical_record_uuid",
                table: "clinical_records",
                column: "clinical_record_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_password_recovery_tokens_token_hash",
                table: "password_recovery_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "password_recovery_tokens");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "i_x_prescriptions_prescription_uuid",
                table: "prescriptions");

            migrationBuilder.DropIndex(
                name: "i_x_patients_customer_uuid",
                table: "patients");

            migrationBuilder.DropIndex(
                name: "i_x_clinical_records_clinical_record_uuid",
                table: "clinical_records");

            migrationBuilder.AlterColumn<string>(
                name: "prescription_uuid",
                table: "prescriptions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "customer_uuid",
                table: "patients",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "birth_date",
                table: "patients",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "clinical_record_uuid",
                table: "clinical_records",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);
        }
    }
}
