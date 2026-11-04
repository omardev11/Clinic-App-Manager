using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppManagerDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingPatientName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PaitentId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaitentId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PaitentId",
                table: "Patients",
                newName: "PatientId");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Patients",
                newName: "PaitentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaitentId",
                table: "Appointments",
                column: "PaitentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PaitentId",
                table: "Appointments",
                column: "PaitentId",
                principalTable: "Patients",
                principalColumn: "PaitentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
