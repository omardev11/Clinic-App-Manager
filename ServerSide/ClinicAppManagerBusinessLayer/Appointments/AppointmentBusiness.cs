using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Globalization;
using System.Security.Claims;

namespace ClinicAppManagerBusinessLayer.Appointments
{
    public class AppointmentBusiness : IAppointmentBusiness
    {
        private readonly IAppointmentData _AppointmentData;
        private readonly IAppointmentStatusData _AppointmentStatusData;
        private readonly IDoctorData _DoctorData;
        private readonly IPatientData _PatientData;
        private string? _userRole;
        public AppointmentDTO Appointment { get; set; }

        private SecurityHelper.EnMode Mode;

        public AppointmentBusiness(
            IAppointmentData AppointmentData,
            IDoctorData DoctorData,
            IPatientData PatientData,
            IAppointmentStatusData appointmentStatusData)
        {
            _AppointmentData = AppointmentData;
            Appointment = new AppointmentDTO();
            _DoctorData = DoctorData;
            _PatientData = PatientData;
            _AppointmentStatusData = appointmentStatusData;
        }

        public void Initialize(
            AppointmentDTO Appointment,
            string? userRole = null,
            SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.Appointment = Appointment;
            if (_userRole == null)
                _userRole = userRole;
            Mode = mode;
        }

        private async Task<bool> _AddNewAppointment()
        {
            if (Appointment == null)
            {
                return false;
            }

            var appointmentEntity = new Appointment
            {
                AppointmentDate = Appointment.AppointmentDate!.Value,
                AppointmentTime = Appointment.AppointmentTime!.Value,
                AppointmentStatusId = Appointment.AppointmentStatusId ?? 1,
                DoctorId = Appointment.DoctorId!.Value,
                PatientId = Appointment.PatientId!.Value
            };

            Appointment.AppointmentId = await _AppointmentData.AddNewAppointment(appointmentEntity);
            return Appointment.AppointmentId.HasValue;
        }

        private async Task<bool> _UpdateAppointment()
        {
            if (!Appointment.AppointmentId.HasValue)
            {
                return false;
            }

            return await _AppointmentData.UpdateAppointment(Appointment);
        }

        public async Task<DetailAppointmentDTO> GetAppointmentById(int AppointmentId)
        {
            if (AppointmentId <= 0)
            {
                return null;
            }

            var entityAppointment = await _AppointmentData.GetAppointmentByAppointmentId(AppointmentId);
            return await MapToDetailAppointmentDto(entityAppointment);
        }

        public async Task<List<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentDate(DateOnly AppointmentDate)
        {
            var entityAppointments = await _AppointmentData.GetAppointmentInfoByAppointmentDate(AppointmentDate);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentTime(TimeOnly AppointmentTime)
        {
            var entityAppointments = await _AppointmentData.GetAppointmentInfoByAppointmentTime(AppointmentTime);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAppointmentInfoBetweenTimes(TimeOnly StartTime, TimeOnly EndTime)
        {
            var entityAppointments = await _AppointmentData.GetAppointmentInfoBetweenTimes(StartTime, EndTime);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAppointmentInfoBetweenDates(DateOnly StartDate, DateOnly EndDate)
        {
            var entityAppointments = await _AppointmentData.GetAppointmentInfoBetweenDates(StartDate, EndDate);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAllAppointmentsForThisDoctor(int DoctorId)
        {
            if (DoctorId <= 0)
            {
                return null;
            }

            var entityAppointments = await _AppointmentData.GetAllAppointmentsForThisDoctor(DoctorId);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAllAppointmentsForThisPatient(int PatientId)
        {
            if (PatientId <= 0)
            {
                return null;
            }

            var entityAppointments = await _AppointmentData.GetAllAppointmentsForThisPatient(PatientId);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAllAppointmentsBetweenThisDoctorAndPatient(int DoctorId, int PatientId)
        {
            if (DoctorId <= 0 || PatientId <= 0)
            {
                return null;
            }

            var entityAppointments = await _AppointmentData.GetAllAppointmentsBetweenThisDoctorAndPatient(DoctorId, PatientId);
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<List<DetailAppointmentDTO>> GetAllAppointmentes()
        {
            var entityAppointments = await _AppointmentData.GetAllAppointments();
            return await MapToDetailAppointmentDtos(entityAppointments);
        }

        public async Task<bool> HasConflictingAppointment(
            int doctorId,
            int patientId,
            DateOnly appointmentDate,
            TimeOnly appointmentTime,
            int? excludeAppointmentId = null)
        {
            return await _AppointmentData.HasConflictingAppointment(
                doctorId,
                patientId,
                appointmentDate,
                appointmentTime,
                excludeAppointmentId);
        }

        public string GetCurrentAppointmentRole(IEnumerable<string> roles)
        {
            var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (roleSet.Contains("user"))
            {
                return "user";
            }

            if (roleSet.Contains("doctor"))
            {
                return "doctor";
             
            }

            if (roleSet.Contains("patient"))
            {
                return "patient";
            }

            return string.Empty;
        }

        public async Task<bool> CheckIfThisPatientHasRoleOnThisAppointment(int patientId, int appointmentId)
        {
            if (patientId <= 0 || appointmentId <= 0)
            {
                return false;
            }

            return await _AppointmentData.CheckIfThisPatientHasRoleOnThisAppointment(patientId, appointmentId);
        }

        public async Task<bool> CheckIfThisDoctorHasRoleOnThisAppointment(int doctorId, int appointmentId)
        {
            if (doctorId <= 0 || appointmentId <= 0)
            {
                return false;
            }

            return await _AppointmentData.CheckIfThisDoctorHasRoleOnThisAppointment(doctorId, appointmentId);
        }

        public async Task<bool> CheckingPatientIfExist(IEnumerable<string> roles,string AspnetUserId)
        {
            var userRole = GetCurrentAppointmentRole(roles);
            _userRole = userRole; // Store the role for later use in mapping DTOs.
          
            if (userRole == "patient")
            {
                var existingPatient = await _PatientData.GetPatientInfoByAspNetUserId(AspnetUserId);
                if (existingPatient == null)
                {
                   return false;
                }
            }
            return true;
        }
        public async Task<bool> CheckingDoctorIfExist(IEnumerable<string> roles, string AspnetUserId)
        {
            var userRole = GetCurrentAppointmentRole(roles);
            _userRole = userRole; // Store the role for later use in mapping DTOs.

            if (userRole == "doctor")
            {
                var existingDoctor = await _DoctorData.GetDoctorInfoByAspNetUserId(AspnetUserId);
                if (existingDoctor == null)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> PatientHasAccess(string userId, int appointmentId)
        {
            var patient = await _PatientData.GetPatientInfoByAspNetUserId(userId);
            if (patient == null) return false;

            return await _AppointmentData.CheckIfThisPatientHasRoleOnThisAppointment(patient.PatientId, appointmentId);
        }
        private async Task<bool> DoctorHasAccess(string userId, int appointmentId)
        {
            var doctor = await _DoctorData.GetDoctorInfoByAspNetUserId(userId);
            if (doctor == null) return false;

            return await _AppointmentData.CheckIfThisPatientHasRoleOnThisAppointment(doctor.DoctorId, appointmentId);
        }

        public async Task<bool> HasAccessToAppointmentAsync(IEnumerable<string> roles, string AspnetUserId,
                                                                            int AppoinmentId)
        {
            var userRole = GetCurrentAppointmentRole(roles);
            _userRole = userRole; // Store the role for later use in mapping DTOs.

            return roles switch
            {
                 _ when userRole == "patient" => await PatientHasAccess(AspnetUserId, AppoinmentId),
                 _ when userRole == "doctor" => await DoctorHasAccess(AspnetUserId, AppoinmentId),
                _ => false
            };

             
        }

        public async Task<List<DetailAppointmentDTO>> FilterAppointmentsByOwnership(List<DetailAppointmentDTO> appointments,
                                                   IEnumerable<string> roles, string AspnetUserId)
        {
            string userRole = GetCurrentAppointmentRole(roles);
            if (appointments == null || !appointments.Any() || userRole == "user")
            {
                return appointments;
            }

            var allowed = new List<DetailAppointmentDTO>();
            if (userRole == "patient")
            {

                var patient = await _PatientData.GetPatientInfoByAspNetUserId(AspnetUserId);
                if (patient == null)
                {
                    return new List<DetailAppointmentDTO>();
                }

                foreach (var appointment in appointments)
                {
                    bool hasAccess = await _AppointmentData.CheckIfThisPatientHasRoleOnThisAppointment(
                        patient.PatientId,
                        appointment.Appointment.AppointmentId!.Value);
                    if (hasAccess)
                    {
                        allowed.Add(appointment);
                    }
                }

                return allowed;
            }

            if (userRole == "doctor")
            {
                var doctor = await _DoctorData.GetDoctorInfoByAspNetUserId(AspnetUserId);
                if (doctor == null)
                {
                    return new List<DetailAppointmentDTO>();
                }

                foreach (var appointment in appointments)
                {
                    bool hasAccess = await _AppointmentData.CheckIfThisDoctorHasRoleOnThisAppointment(
                        doctor.DoctorId,
                        appointment.Appointment.AppointmentId!.Value);
                    if (hasAccess)
                    {
                        allowed.Add(appointment);
                    }
                }

                return allowed;
            }

            return new List<DetailAppointmentDTO>();
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewAppointment();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdateAppointment();
                default:
                    return false;
            }
        }

        private async Task<List<DetailAppointmentDTO>> MapToDetailAppointmentDtos(List<Appointment> entityAppointments)
        {
            if (entityAppointments == null || !entityAppointments.Any())
            {
                return new List<DetailAppointmentDTO>();
            }

            var tasks = entityAppointments.Select(MapToDetailAppointmentDto);
            return (await Task.WhenAll(tasks)).ToList();
        }

        private async Task<DetailAppointmentDTO> MapToDetailAppointmentDto(Appointment entityAppointment)
        {
            if (entityAppointment == null)
            {
                return null;
            }

            var patient = await _PatientData.GetPatientInfoById(entityAppointment.PatientId);
            var doctor = await _DoctorData.GetDoctorInfoById(entityAppointment.DoctorId);

            var detailAppointment = new DetailAppointmentDTO
            {
                Appointment = new AppointmentDTO
                {
                    AppointmentId = entityAppointment.AppointmentId,
                    PatientId = entityAppointment.PatientId,
                    DoctorId = entityAppointment.DoctorId,
                    AppointmentDate = entityAppointment.AppointmentDate,
                    AppointmentTime = entityAppointment.AppointmentTime,
                    AppointmentStatusId = entityAppointment.AppointmentStatusId
                },
                AppointmentStatus = await _AppointmentStatusData.GetAppointmentStatusById(entityAppointment.AppointmentStatusId)
            };

            // Limit names by caller role.
            switch (_userRole)
            {
                case "patient":
                    detailAppointment.DoctorFullName = doctor?.Person?.FullName ?? string.Empty;
                    break;
                case "doctor":
                    detailAppointment.PatientFullName = patient?.Person?.FullName ?? string.Empty;
                    break;
                default:
                    detailAppointment.PatientFullName = patient?.Person?.FullName ?? string.Empty;
                    detailAppointment.DoctorFullName = doctor?.Person?.FullName ?? string.Empty;
                    break;
            }

            return detailAppointment;
        }
    }
}
