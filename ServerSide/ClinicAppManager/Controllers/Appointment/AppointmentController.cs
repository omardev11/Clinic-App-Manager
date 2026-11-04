using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.Appointments;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicAppManager.Controllers.Appointment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentBusiness _AppointmentBusiness;
        private readonly IPatientBusiness _PatientBusiness;
        private readonly IDoctorBusiness _DoctorBusiness;
        private readonly IAppointmentStatusBusiness _AppointmentStatusBusiness;

        public AppointmentController(
            IAppointmentBusiness AppointmentBusiness,
            IPatientBusiness patientBusiness,
            IDoctorBusiness doctorBusiness,
            IAppointmentStatusBusiness appointmentStatusBusiness)
        {
            _AppointmentBusiness = AppointmentBusiness;
            _PatientBusiness = patientBusiness;
            _DoctorBusiness = doctorBusiness;
            _AppointmentStatusBusiness = appointmentStatusBusiness;
        }

        [Authorize(Roles = "patient,user")]
        [HttpPost(Name = "AddAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewAppointment(AppointmentDTO NewAppointment)
        {
            if (NewAppointment == null)
            {
                return BadRequest("The Object Of The Appointment Can Not Be Null");
            }
          

            bool result = await _AppointmentBusiness.CheckingPatientIfExist(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                                  User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!result)
            {
                return Forbid();
            }
                _AppointmentBusiness.Initialize(NewAppointment);
            if (await _AppointmentBusiness.Save())
            {
                return Ok(_AppointmentBusiness.Appointment.AppointmentId);
            }

            return StatusCode(500, new { messege = "Error Adding New Appointment" });
        }

        [Authorize(Roles = "user")]
        [HttpPut("Update", Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdateAppointment(
            int AppointmentId,
            [FromQuery] int? PatientId,
            [FromQuery] int? DoctorId,
            [FromQuery] DateOnly? AppointmentDate,
            [FromQuery] TimeOnly? AppointmentTime,
            [FromQuery] int? AppointmentStatusId)
        {
            if (AppointmentId <= 0)
            {
                return BadRequest("The Appointment Id must be a valid number.");
            }

            var existingAppointment = await _AppointmentBusiness.GetAppointmentById(AppointmentId);
            if (existingAppointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            AppointmentDTO appointmentDTO = new()
            {
                AppointmentId = AppointmentId,
                PatientId = PatientId,
                DoctorId = DoctorId,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime,
                AppointmentStatusId = AppointmentStatusId
            };

            _AppointmentBusiness.Initialize(
                appointmentDTO,
                _AppointmentBusiness.GetCurrentAppointmentRole(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value)),
                SecurityHelper.EnMode.UpdateMode);
            if (await _AppointmentBusiness.Save())
            {
                return Ok("Appointment Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating Appointment Data" });
        }

        [Authorize(Roles = "patient")]
        [HttpPut("Reschedule", Name = "RescheduleAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> RescheduleAppointment(
            int AppointmentId,
            DateOnly? AppointmentDate,
            TimeOnly? AppointmentTime)
        {
            if (AppointmentId <= 0)
            {
                return BadRequest("The Appointment Id must be a valid number.");
            }

            if (!AppointmentDate.HasValue && !AppointmentTime.HasValue)
            {
                return BadRequest("Appointment date or time is required.");
            }

            var existingAppointment = await _AppointmentBusiness.GetAppointmentById(AppointmentId);
            if (existingAppointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                          User.FindFirstValue(ClaimTypes.NameIdentifier), AppointmentId);

          
            if (!hasAccess)
            {
                return Forbid();
            }

            DateOnly targetDate = AppointmentDate ?? existingAppointment.Appointment.AppointmentDate!.Value;
            TimeOnly targetTime = AppointmentTime ?? existingAppointment.Appointment.AppointmentTime!.Value;

            // Avoid near-duplicate times for the same doctor/patient pair.
            bool hasConflict = await _AppointmentBusiness.HasConflictingAppointment(
                existingAppointment.Appointment.DoctorId!.Value,
                existingAppointment.Appointment.PatientId!.Value,
                targetDate,
                targetTime,
                AppointmentId);

            if (hasConflict)
            {
                return Conflict(new
                {
                    error = "There is another appointment for this doctor and patient within 20 minutes of the selected time."
                });
            }

            AppointmentDTO appointmentDTO = new()
            {
                AppointmentId = AppointmentId,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime
            };

            _AppointmentBusiness.Initialize(appointmentDTO, "patient", SecurityHelper.EnMode.UpdateMode);
            if (await _AppointmentBusiness.Save())
            {
                return Ok("Appointment updated successfully.");
            }

            return StatusCode(500, new { messege = "Error Updating Appointment Data" });
        }

        [Authorize(Roles = "patient")]
        [HttpPut("Cancel", Name = "CancelAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CancelAppointment(int AppointmentId)
        {
            if (AppointmentId <= 0)
            {
                return BadRequest("The Appointment Id must be a valid number.");
            }

            var existingAppointment = await _AppointmentBusiness.GetAppointmentById(AppointmentId);
            if (existingAppointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                          User.FindFirstValue(ClaimTypes.NameIdentifier), AppointmentId);
           
            if (!hasAccess)
            {
                return Forbid();
            }

            // Resolve the cancel status on the server.
            var canceledStatus = await _AppointmentStatusBusiness.GetAppointmentStatusByStatusName("Canceled")
                ?? await _AppointmentStatusBusiness.GetAppointmentStatusByStatusName("Cancelled");
            if (canceledStatus == null || !canceledStatus.AppointmentStatusId.HasValue)
            {
                return StatusCode(500, new { message = "Canceled appointment status was not found." });
            }

            AppointmentDTO appointmentDTO = new()
            {
                AppointmentId = AppointmentId,
                AppointmentStatusId = canceledStatus.AppointmentStatusId.Value
            };

            _AppointmentBusiness.Initialize(appointmentDTO, "patient", SecurityHelper.EnMode.UpdateMode);
            if (await _AppointmentBusiness.Save())
            {
                return Ok("Appointment canceled successfully.");
            }

            return StatusCode(500, new { messege = "Error canceling appointment." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ById", Name = "GetAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAppointmentById(int AppointmentId)
        {
            if (AppointmentId <= 0)
            {
                return BadRequest("The Appointment Id must be a valid number.");
            }

            bool result = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
                                                                                          .Select(claim => claim.Value),
                                                          User.FindFirstValue(ClaimTypes.NameIdentifier), AppointmentId);

            if (result)
            {

                _AppointmentBusiness.Initialize(new AppointmentDTO());
                var appointment = await _AppointmentBusiness.GetAppointmentById(AppointmentId);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found." });
                }
                return Ok(appointment);
            }
            else
            {
                return Forbid();
            }
           
            

          
          
        }

        [Authorize(Roles = "doctor,user")]
        [HttpGet("ForDoctor", Name = "GetAllAppointmentsForThisDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAllAppointmentsForThisDoctor(int DoctorId)
        {
           
            if (DoctorId <= 0)
            {
                return BadRequest("The Doctor Id must be a valid number.");
            }

            bool result = await _AppointmentBusiness.CheckingDoctorIfExist(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                                  User.FindFirstValue(ClaimTypes.NameIdentifier));

          if (!result)
            {
                return Forbid();
            }

            _AppointmentBusiness.Initialize(new AppointmentDTO());
            var appointment = await _AppointmentBusiness.GetAllAppointmentsForThisDoctor(DoctorId);
           
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,user")]
        [HttpGet("ForPatient", Name = "GetAllAppointmentsForThisPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAllAppointmentsForThisPatient(int PatientId)
        {
            if (PatientId <= 0)
            {
                return BadRequest("The Patient Id must be a valid number.");
            }

            bool result = await _AppointmentBusiness.CheckingPatientIfExist(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                                  User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (!result)
                return Forbid();

            _AppointmentBusiness.Initialize(new AppointmentDTO());
            var appointment = await _AppointmentBusiness.GetAllAppointmentsForThisPatient(PatientId);
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("BetweenPatineAndDoctor", Name = "GetAllAppointmentsBetweenThisDoctorAndPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAllAppointmentsBetweenThisDoctorAndPatient(int DoctorId, int PatientId)
        {
            if (PatientId <= 0 || DoctorId <= 0)
            {
                return BadRequest("The Doctor Id and Patient Id must be valid numbers.");
            }

          
            bool patientResult = await _AppointmentBusiness.CheckingPatientIfExist(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                                  User.FindFirstValue(ClaimTypes.NameIdentifier));

            bool doctorResult = await _AppointmentBusiness.CheckingDoctorIfExist(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
                                                                         User.FindFirstValue(ClaimTypes.NameIdentifier));

           if (!patientResult && !doctorResult)
            {
                return Forbid();
            }

            _AppointmentBusiness.Initialize(new AppointmentDTO());
            var appointment = await _AppointmentBusiness.GetAllAppointmentsBetweenThisDoctorAndPatient(DoctorId, PatientId);
           
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByDate", Name = "GetAppointmentInfoByAppointmentDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentDate(DateOnly date)
        {
            if (date == DateOnly.MinValue)
            {
                return BadRequest("The Appointment Date must be provided.");
            }

            string userRole = _AppointmentBusiness.GetCurrentAppointmentRole(
                User.FindAll(ClaimTypes.Role).Select(claim => claim.Value));
            _AppointmentBusiness.Initialize(new AppointmentDTO(), userRole);
            var appointment = await _AppointmentBusiness.GetAppointmentInfoByAppointmentDate(date);
            appointment = await _AppointmentBusiness.FilterAppointmentsByOwnership(appointment, User.FindAll(ClaimTypes.Role)
                                            .Select(claim => claim.Value),User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByTime", Name = "GetAppointmentInfoByAppointmentTime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAppointmentInfoByAppointmentTime(TimeOnly time)
        {
            if (time == TimeOnly.MinValue)
            {
                return BadRequest("The Appointment Time must be provided.");
            }

            string userRole = _AppointmentBusiness.GetCurrentAppointmentRole(
                User.FindAll(ClaimTypes.Role).Select(claim => claim.Value));
            _AppointmentBusiness.Initialize(new AppointmentDTO(), userRole);
            var appointment = await _AppointmentBusiness.GetAppointmentInfoByAppointmentTime(time);
            appointment = await _AppointmentBusiness.FilterAppointmentsByOwnership(appointment, User.FindAll(ClaimTypes.Role)
                                              .Select(claim => claim.Value),User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("BetweenTimes", Name = "GetAppointmentInfoBetweenTimes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAppointmentInfoBetweenTimes(TimeOnly StartTime, TimeOnly EndTime)
        {
            if (StartTime == TimeOnly.MinValue || EndTime == TimeOnly.MinValue)
            {
                return BadRequest("The Appointment Time range must be provided.");
            }

            string userRole = _AppointmentBusiness.GetCurrentAppointmentRole(
                User.FindAll(ClaimTypes.Role).Select(claim => claim.Value));
            _AppointmentBusiness.Initialize(new AppointmentDTO(), userRole);
            var appointment = await _AppointmentBusiness.GetAppointmentInfoBetweenTimes(StartTime, EndTime);
            appointment = await _AppointmentBusiness.FilterAppointmentsByOwnership(appointment, User.FindAll(ClaimTypes.Role)
                                              .Select(claim => claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("BetweenDates", Name = "GetAppointmentInfoBetweenDates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAppointmentInfoBetweenDates(DateOnly StartDate, DateOnly EndDate)
        {
            if (StartDate == DateOnly.MinValue || EndDate == DateOnly.MinValue)
            {
                return BadRequest("The Appointment Date range must be provided.");
            }

            string userRole = _AppointmentBusiness.GetCurrentAppointmentRole(
                User.FindAll(ClaimTypes.Role).Select(claim => claim.Value));
            _AppointmentBusiness.Initialize(new AppointmentDTO(), userRole);
            var appointment = await _AppointmentBusiness.GetAppointmentInfoBetweenDates(StartDate, EndDate);
            appointment = await _AppointmentBusiness.FilterAppointmentsByOwnership(appointment, User.FindAll(ClaimTypes.Role)
                                              .Select(claim => claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllAppointmentes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailAppointmentDTO>> GetAllAppointmentes()
        {
            _AppointmentBusiness.Initialize(
                new AppointmentDTO(),
                _AppointmentBusiness.GetCurrentAppointmentRole(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value)));
            var appointment = await _AppointmentBusiness.GetAllAppointmentes();
            if (appointment != null)
            {
                return Ok(appointment);
            }

            return NotFound(new { message = "Appointment not found." });
        }

    }
}
