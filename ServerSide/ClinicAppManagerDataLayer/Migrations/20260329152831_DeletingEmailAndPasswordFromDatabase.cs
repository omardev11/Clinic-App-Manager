using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppManagerDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class DeletingEmailAndPasswordFromDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AspNetUserId",
                table: "Patients",
                column: "AspNetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_AspNetUserId",
                table: "Patients",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_AspNetUserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AspNetUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
