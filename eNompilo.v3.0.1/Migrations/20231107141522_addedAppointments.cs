using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eNompilo.v3._0._1.Migrations
{
    public partial class addedAppointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CounsellingAppointmentId",
                table: "Session",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FamilyPlanningAppointmentId",
                table: "Session",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneralAppointmentId",
                table: "Session",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VaccinationAppointmentId",
                table: "Session",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_CounsellingAppointmentId",
                table: "Session",
                column: "CounsellingAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_FamilyPlanningAppointmentId",
                table: "Session",
                column: "FamilyPlanningAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_GeneralAppointmentId",
                table: "Session",
                column: "GeneralAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_VaccinationAppointmentId",
                table: "Session",
                column: "VaccinationAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_CounsellingAppointment_CounsellingAppointmentId",
                table: "Session",
                column: "CounsellingAppointmentId",
                principalTable: "CounsellingAppointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_FamilyPlanningAppointment_FamilyPlanningAppointmentId",
                table: "Session",
                column: "FamilyPlanningAppointmentId",
                principalTable: "FamilyPlanningAppointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_GeneralAppointment_GeneralAppointmentId",
                table: "Session",
                column: "GeneralAppointmentId",
                principalTable: "GeneralAppointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_VaccinationAppointment_VaccinationAppointmentId",
                table: "Session",
                column: "VaccinationAppointmentId",
                principalTable: "VaccinationAppointment",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_CounsellingAppointment_CounsellingAppointmentId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_FamilyPlanningAppointment_FamilyPlanningAppointmentId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_GeneralAppointment_GeneralAppointmentId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_VaccinationAppointment_VaccinationAppointmentId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_CounsellingAppointmentId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_FamilyPlanningAppointmentId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_GeneralAppointmentId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_VaccinationAppointmentId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "CounsellingAppointmentId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "FamilyPlanningAppointmentId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "GeneralAppointmentId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "VaccinationAppointmentId",
                table: "Session");
        }
    }
}
