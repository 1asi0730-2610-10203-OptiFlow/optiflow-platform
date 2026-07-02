using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryEntityUseProductCategoryEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "products");

            // Normalize existing free-text category values into the new EProductCategory
            // enum's uppercase string representation (see ModelBuilderExtensions.HasConversion).
            // Anything unrecognized falls back to LENSES and should be reviewed manually.
            migrationBuilder.Sql(@"UPDATE products SET category = CASE category
                WHEN 'Lunas' THEN 'LENSES'
                WHEN 'Armazones' THEN 'FRAMES'
                WHEN 'Accesorios' THEN 'ACCESSORIES'
                WHEN 'Lentes de Contacto' THEN 'CONTACTLENSES'
                WHEN 'Lentes de Sol' THEN 'SUNGLASSES'
                WHEN 'Equipos' THEN 'EQUIPMENT'
                ELSE 'LENSES'
            END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_categories", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}
