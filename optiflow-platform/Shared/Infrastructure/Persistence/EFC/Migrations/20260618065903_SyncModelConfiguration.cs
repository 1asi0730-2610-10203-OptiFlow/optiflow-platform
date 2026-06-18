using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "phone",
                table: "suppliers",
                newName: "contact_phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "suppliers",
                newName: "contact_email");

            migrationBuilder.AddColumn<string>(
                name: "prescription_uuid",
                table: "prescriptions",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "customer_uuid",
                table: "patients",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "clinical_record_uuid",
                table: "clinical_records",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "i_x_suppliers_name",
                table: "suppliers",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_suppliers_name",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "prescription_uuid",
                table: "prescriptions");

            migrationBuilder.DropColumn(
                name: "customer_uuid",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "clinical_record_uuid",
                table: "clinical_records");

            migrationBuilder.RenameColumn(
                name: "contact_phone",
                table: "suppliers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "contact_email",
                table: "suppliers",
                newName: "email");
        }
    }
}
