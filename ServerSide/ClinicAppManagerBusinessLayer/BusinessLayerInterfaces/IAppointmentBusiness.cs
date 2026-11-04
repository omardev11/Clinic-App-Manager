using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClinicAppManagerBusinessLayer.Appointments.AppointmentBusiness;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IAppointmentBusiness
    {
        public void Initialize(
            AppointmentDTO Appointment,
            string? userRole = null,
            SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        public AppointmentDTO Appointment { get; set; }
        public Task<List<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentDate(DateOnly AppointmentDate);
        public Task<List<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentTime(TimeOnly AppointmentTime);
        public Task<List<DetailAppointmentDTO>> GetAppointmentInfoBetweenTimes(TimeOnly StartTime, TimeOnly EndTime);
        public Task<List<DetailAppointmentDTO>> GetAppointmentInfoBetweenDates(DateOnly StartDate, DateOnly EndDate);
        public Task<List<DetailAppointmentDTO>> GetAllAppointmentsForThisDoctor(int DoctorId);
        public Task<List<DetailAppointmentDTO>> GetAllAppointmentsForThisPatient(int PatientId);
        public Task<List<DetailAppointmentDTO>> GetAllAppointmentsBetweenThisDoctorAndPatient(int DoctorId, int PatientId);
        public Task<List<DetailAppointmentDTO>> GetAllAppointmentes();
        public Task<bool> HasConflictingAppointment(int doctorId, int patientId, DateOnly appointmentDate, TimeOnly appointmentTime, int? excludeAppointmentId = null);
        public string GetCurrentAppointmentRole(IEnumerable<string> roles);
        public Task<bool> CheckIfThisPatientHasRoleOnThisAppointment(int patientId, int appointmentId);
        public Task<bool> CheckIfThisDoctorHasRoleOnThisAppointment(int doctorId, int appointmentId);
        public Task<bool> HasAccessToAppointmentAsync(IEnumerable<string> roles, string AspnetUserId,
                                                                           int AppoinmentId);
        public Task<List<DetailAppointmentDTO>> FilterAppointmentsByOwnership(List<DetailAppointmentDTO> appointments,
                                                   IEnumerable<string> roles, string AspnetUserId);

        public Task<bool> CheckingDoctorIfExist(IEnumerable<string> roles, string AspnetUserId);
        public Task<bool> CheckingPatientIfExist(IEnumerable<string> roles, string AspnetUserId);
        public Task<bool> Save();

        public Task<DetailAppointmentDTO> GetAppointmentById(int AppointmentId);

    }
}
