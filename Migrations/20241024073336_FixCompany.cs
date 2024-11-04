using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVRecruitment.Migrations
{
    /// <inheritdoc />
    public partial class FixCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailCompany",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hotline",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailCompany",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Hotline",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Company");
        }
    }
}
