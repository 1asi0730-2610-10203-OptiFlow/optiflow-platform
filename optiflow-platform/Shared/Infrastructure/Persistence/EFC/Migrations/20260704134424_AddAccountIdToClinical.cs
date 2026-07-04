using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToClinical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_patients_dni",
                table: "patients");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "prescriptions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "patients",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "clinical_records",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "i_x_patients_account_id_dni",
                table: "patients",
                columns: new[] { "account_id", "dni" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_patients_account_id_dni",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "prescriptions");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "clinical_records");

            migrationBuilder.CreateIndex(
                name: "i_x_patients_dni",
                table: "patients",
                column: "dni",
                unique: true);
        }
    }
}
