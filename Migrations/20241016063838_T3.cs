using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVRecruitment.Migrations
{
    /// <inheritdoc />
    public partial class T3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmCompany",
                table: "Company",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmCompany",
                table: "Company");
        }
    }
}
