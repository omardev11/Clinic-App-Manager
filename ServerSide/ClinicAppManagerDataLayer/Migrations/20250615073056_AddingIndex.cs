using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppManagerDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                computedColumnSql: "FirstName + ' ' + LastName",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_DateOfBirth",
                table: "Persons",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_FullName",
                table: "Persons",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_DateOfBirth",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_FullName",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Persons");
        }
    }
}
