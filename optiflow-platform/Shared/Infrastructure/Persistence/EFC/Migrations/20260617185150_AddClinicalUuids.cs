using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalUuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── patients: add customer_uuid ───────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "customer_uuid",
                table: "patients",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Backfill existing rows with a generated UUID
            migrationBuilder.Sql(
                "UPDATE patients SET customer_uuid = CONCAT('pat-', REPLACE(UUID(), '-', '')) WHERE customer_uuid = ''");

            migrationBuilder.CreateIndex(
                name: "ix_patients_customer_uuid",
                table: "patients",
                column: "customer_uuid",
                unique: true);

            // ── clinical_records: add clinical_record_uuid ────────────────
            migrationBuilder.AddColumn<string>(
                name: "clinical_record_uuid",
                table: "clinical_records",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE clinical_records SET clinical_record_uuid = CONCAT('record-', REPLACE(UUID(), '-', '')) WHERE clinical_record_uuid = ''");

            migrationBuilder.CreateIndex(
                name: "ix_clinical_records_clinical_record_uuid",
                table: "clinical_records",
                column: "clinical_record_uuid",
                unique: true);

            // ── prescriptions: add prescription_uuid ─────────────────────
            migrationBuilder.AddColumn<string>(
                name: "prescription_uuid",
                table: "prescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE prescriptions SET prescription_uuid = CONCAT('presc-', REPLACE(UUID(), '-', '')) WHERE prescription_uuid = ''");

            migrationBuilder.CreateIndex(
                name: "ix_prescriptions_prescription_uuid",
                table: "prescriptions",
                column: "prescription_uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_prescriptions_prescription_uuid",
                table: "prescriptions");

            migrationBuilder.DropColumn(
                name: "prescription_uuid",
                table: "prescriptions");

            migrationBuilder.DropIndex(
                name: "ix_clinical_records_clinical_record_uuid",
                table: "clinical_records");

            migrationBuilder.DropColumn(
                name: "clinical_record_uuid",
                table: "clinical_records");

            migrationBuilder.DropIndex(
                name: "ix_patients_customer_uuid",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "customer_uuid",
                table: "patients");
        }
    }
}