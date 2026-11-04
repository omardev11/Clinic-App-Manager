using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppManagerDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheNameOfAppoinmentStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppoinmentStatuses_AppoinmentStatusId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "AppoinmentStatuses");

            migrationBuilder.RenameColumn(
                name: "AppoinmentTime",
                table: "Appointments",
                newName: "AppointmentTime");

            migrationBuilder.RenameColumn(
                name: "AppoinmentStatusId",
                table: "Appointments",
                newName: "AppointmentStatusId");

            migrationBuilder.RenameColumn(
                name: "AppoinmentDate",
                table: "Appointments",
                newName: "AppointmentDate");

            migrationBuilder.RenameColumn(
                name: "AppoinmentId",
                table: "Appointments",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_AppoinmentStatusId",
                table: "Appointments",
                newName: "IX_Appointments_AppointmentStatusId");

            migrationBuilder.CreateTable(
                name: "AppointmentStatuses",
                columns: table => new
                {
                    AppointmentStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentStatuses", x => x.AppointmentStatusId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentStatuses_AppointmentStatusId",
                table: "Appointments",
                column: "AppointmentStatusId",
                principalTable: "AppointmentStatuses",
                principalColumn: "AppointmentStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentStatuses_AppointmentStatusId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentStatuses");

            migrationBuilder.RenameColumn(
                name: "AppointmentTime",
                table: "Appointments",
                newName: "AppoinmentTime");

            migrationBuilder.RenameColumn(
                name: "AppointmentStatusId",
                table: "Appointments",
                newName: "AppoinmentStatusId");

            migrationBuilder.RenameColumn(
                name: "AppointmentDate",
                table: "Appointments",
                newName: "AppoinmentDate");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Appointments",
                newName: "AppoinmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_AppointmentStatusId",
                table: "Appointments",
                newName: "IX_Appointments_AppoinmentStatusId");

            migrationBuilder.CreateTable(
                name: "AppoinmentStatuses",
                columns: table => new
                {
                    AppoinmentStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppoinmentStatuses", x => x.AppoinmentStatusId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppoinmentStatuses_AppoinmentStatusId",
                table: "Appointments",
                column: "AppoinmentStatusId",
                principalTable: "AppoinmentStatuses",
                principalColumn: "AppoinmentStatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
