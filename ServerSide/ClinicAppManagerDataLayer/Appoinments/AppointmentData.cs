using ClinicAppManagerDataLayer.DataLayerInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer.Appoinments
{
    public class AppointmentData : IAppointmentData
    {
        private readonly AppDbContext _context;

        public AppointmentData(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int?> AddNewAppointment(Entities.Appointment Appointment)
        {
            await _context.Appointments.AddAsync(Appointment);
            await _context.SaveChangesAsync();

            return Appointment.AppointmentId;
        }

        public async Task<bool> UpdateAppointment(DTO.AppointmentDTO Appointment)
        {
            var existingAppointment = await _context.Appointments.FindAsync(Appointment.AppointmentId);
            if (existingAppointment == null)
            {
                return false;
            }

            existingAppointment.PatientId = Appointment.PatientId ?? existingAppointment.PatientId;
            existingAppointment.DoctorId = Appointment.DoctorId ?? existingAppointment.DoctorId;
            existingAppointment.AppointmentDate = Appointment.AppointmentDate ?? existingAppointment.AppointmentDate;
            existingAppointment.AppointmentTime = Appointment.AppointmentTime ?? existingAppointment.AppointmentTime;
            existingAppointment.AppointmentStatusId = Appointment.AppointmentStatusId ?? existingAppointment.AppointmentStatusId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entities.Appointment> GetAppointmentByAppointmentId(int AppointmentId)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(p => p.AppointmentId == AppointmentId);
        }

        public async Task<List<Entities.Appointment>> GetAppointmentInfoByAppointmentDate(DateOnly AppointmentDate)
        {
            return await _context.Appointments
                .Where(p => p.AppointmentDate == AppointmentDate)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAppointmentInfoByAppointmentTime(TimeOnly AppointmentTime)
        {
            return await _context.Appointments
                .Where(p => p.AppointmentTime == AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAppointmentInfoBetweenDates(DateOnly StartDate, DateOnly EndDate)
        {
            return await _context.Appointments
                .Where(p => p.AppointmentDate >= StartDate && p.AppointmentDate <= EndDate)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAppointmentInfoBetweenTimes(TimeOnly StartTime, TimeOnly EndTime)
        {
            return await _context.Appointments
                .Where(p => p.AppointmentTime >= StartTime && p.AppointmentTime <= EndTime)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAllAppointmentsForThisPatient(int PatientId)
        {
            return await _context.Appointments
                .Where(p => p.PatientId == PatientId)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAllAppointmentsForThisDoctor(int DoctorId)
        {
            return await _context.Appointments
                .Where(p => p.DoctorId == DoctorId)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAllAppointmentsBetweenThisDoctorAndPatient(int DoctorId, int PatientId)
        {
            return await _context.Appointments
                .Where(p => p.DoctorId == DoctorId && p.PatientId == PatientId)
                .ToListAsync();
        }

        public async Task<List<Entities.Appointment>> GetAllAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<bool> HasConflictingAppointment(
            int doctorId,
            int patientId,
            DateOnly appointmentDate,
            TimeOnly appointmentTime,
            int? excludeAppointmentId = null)
        {
            var appointments = await _context.Appointments
                .Where(p =>
                    p.DoctorId == doctorId &&
                    p.PatientId == patientId &&
                    p.AppointmentDate == appointmentDate &&
                    (!excludeAppointmentId.HasValue || p.AppointmentId != excludeAppointmentId.Value))
                .ToListAsync();

            return appointments.Any(p =>
                Math.Abs((p.AppointmentTime.ToTimeSpan() - appointmentTime.ToTimeSpan()).TotalMinutes) <= 20);
        }
        public async Task<bool> CheckIfThisPatientHasRoleOnThisAppointment(int patientId, int appointmentId)
        {
            return await _context.Appointments
                .AnyAsync(p => p.PatientId == patientId && p.AppointmentId == appointmentId);
        }

        public async Task<bool> CheckIfThisDoctorHasRoleOnThisAppointment(int doctorId, int appointmentId)
        {
            return await _context.Appointments
                .AnyAsync(p => p.DoctorId == doctorId && p.AppointmentId == appointmentId);
        }

    }




}
