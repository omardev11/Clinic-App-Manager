using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IAppointmentData
    {
        public Task<int?> AddNewAppointment(Entities.Appointment Appointment);
        public Task<bool> UpdateAppointment(DTO.AppointmentDTO Appointment);
        public Task<Entities.Appointment> GetAppointmentByAppointmentId(int AppointmentId);
        public Task<List<Entities.Appointment>> GetAppointmentInfoByAppointmentDate(DateOnly AppointmentDate);
        public Task<List<Entities.Appointment>> GetAppointmentInfoByAppointmentTime(TimeOnly AppointmentTime);
        public Task<List<Entities.Appointment>> GetAppointmentInfoBetweenTimes(TimeOnly StartTime, TimeOnly EndTime);
        public Task<List<Entities.Appointment>> GetAppointmentInfoBetweenDates(DateOnly StartDate, DateOnly EndDate);
        public Task<List<Entities.Appointment>> GetAllAppointmentsForThisPatient(int PatientId);
        public Task<List<Entities.Appointment>> GetAllAppointmentsForThisDoctor(int doctorId);
        public Task<List<Entities.Appointment>> GetAllAppointmentsBetweenThisDoctorAndPatient(int doctorId, int PatientId);

        public Task<List<Entities.Appointment>> GetAllAppointments();
        public Task<bool> HasConflictingAppointment(int doctorId, int patientId, DateOnly appointmentDate, TimeOnly appointmentTime, int? excludeAppointmentId = null);
        public Task<bool> CheckIfThisPatientHasRoleOnThisAppointment(int patientId, int appointmentId);
        public Task<bool> CheckIfThisDoctorHasRoleOnThisAppointment(int doctorId, int appointmentId);
    }
    
}
