using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analytics_reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    generated_by = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    period = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false),
                    generated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    total_revenue = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    total_transactions = table.Column<int>(type: "int", nullable: false),
                    conversion_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    average_delivery_days = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    on_time_delivery_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    rework_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    total_orders = table.Column<int>(type: "int", nullable: false),
                    pending_balance0_to7 = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    pending_balance8_to15 = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    pending_balance16_to30 = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    pending_balance_over30 = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_analytics_reports", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    dni = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_patients", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff_metrics",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    report_id = table.Column<int>(type: "int", nullable: false),
                    employee_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    quotations_issued = table.Column<int>(type: "int", nullable: false),
                    sales_closed = table.Column<int>(type: "int", nullable: false),
                    total_revenue = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_staff_metrics", x => x.id);
                    table.ForeignKey(
                        name: "f_k_staff_metrics_analytics_reports_report_id",
                        column: x => x.report_id,
                        principalTable: "analytics_reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clinical_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    patient_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_clinical_records", x => x.id);
                    table.ForeignKey(
                        name: "f_k_clinical_records_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "prescriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    clinical_record_id = table.Column<int>(type: "int", nullable: false),
                    od_sphere = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    od_cylinder = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    od_axis = table.Column<int>(type: "int", nullable: false),
                    oi_sphere = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    oi_cylinder = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    oi_axis = table.Column<int>(type: "int", nullable: false),
                    addition = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    doctor_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_prescriptions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_prescriptions_clinical_records_clinical_record_id",
                        column: x => x.clinical_record_id,
                        principalTable: "clinical_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_clinical_records_patient_id",
                table: "clinical_records",
                column: "patient_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_patients_dni",
                table: "patients",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_prescriptions_clinical_record_id",
                table: "prescriptions",
                column: "clinical_record_id");

            migrationBuilder.CreateIndex(
                name: "i_x_staff_metrics_report_id",
                table: "staff_metrics",
                column: "report_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prescriptions");

            migrationBuilder.DropTable(
                name: "staff_metrics");

            migrationBuilder.DropTable(
                name: "clinical_records");

            migrationBuilder.DropTable(
                name: "analytics_reports");

            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
