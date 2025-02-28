using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd_TimeManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleColumnEtudiants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Etudiants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Etudiants");
        }
    }
}
