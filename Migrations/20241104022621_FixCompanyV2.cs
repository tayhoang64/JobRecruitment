using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVRecruitment.Migrations
{
    /// <inheritdoc />
    public partial class FixCompanyV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailCompany",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Company",
                newName: "EmailOwner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailOwner",
                table: "Company",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "EmailCompany",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
