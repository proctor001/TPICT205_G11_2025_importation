using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd_TimeManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartementFromEtudiants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departement",
                table: "Etudiants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Departement",
                table: "Etudiants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
