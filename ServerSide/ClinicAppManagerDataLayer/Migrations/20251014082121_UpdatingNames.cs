using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppManagerDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppoinmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethoId",
                table: "Payments");

           

            migrationBuilder.RenameColumn(
                name: "PaymentMethoId",
                table: "Payments",
                newName: "PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "AppoinmentId",
                table: "Payments",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentMethoId",
                table: "Payments",
                newName: "IX_Payments_PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_AppoinmentId",
                table: "Payments",
                newName: "IX_Payments_AppointmentId");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethodId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "Payments",
                newName: "PaymentMethoId");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Payments",
                newName: "AppoinmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                newName: "IX_Payments_PaymentMethoId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_AppointmentId",
                table: "Payments",
                newName: "IX_Payments_AppoinmentId");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PaitentId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppoinmentId",
                table: "Payments",
                column: "AppoinmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethoId",
                table: "Payments",
                column: "PaymentMethoId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
