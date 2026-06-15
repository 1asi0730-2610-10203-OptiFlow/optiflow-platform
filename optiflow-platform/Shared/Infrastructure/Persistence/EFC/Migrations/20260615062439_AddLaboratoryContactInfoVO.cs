using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddLaboratoryContactInfoVO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_info",
                table: "laboratories");

            migrationBuilder.RenameColumn(
                name: "lab_id",
                table: "work_orders",
                newName: "laboratory_id");

            migrationBuilder.AddColumn<string>(
                name: "contact_email",
                table: "laboratories",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "contact_phone",
                table: "laboratories",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_laboratories_name",
                table: "laboratories",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_laboratories_name",
                table: "laboratories");

            migrationBuilder.DropColumn(
                name: "contact_email",
                table: "laboratories");

            migrationBuilder.DropColumn(
                name: "contact_phone",
                table: "laboratories");

            migrationBuilder.RenameColumn(
                name: "laboratory_id",
                table: "work_orders",
                newName: "lab_id");

            migrationBuilder.AddColumn<string>(
                name: "contact_info",
                table: "laboratories",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
