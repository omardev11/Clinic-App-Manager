using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppManager.Controllers.Appointment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentStatusController : ControllerBase
    {
        private readonly IAppointmentStatusBusiness _AppointmentStatusBusiness;

        public AppointmentStatusController(IAppointmentStatusBusiness AppointmentStatusBusiness)
        {
            _AppointmentStatusBusiness = AppointmentStatusBusiness;
        }

        [Authorize(Roles = "user")]
        [HttpPost(Name = "AddAppointmentStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewAppointmentStatus(AppointmentStatusDTO NewAppointmentStatus)
        {
            if (NewAppointmentStatus == null)
            {
                return BadRequest("The Object Of The AppointmentStatus Can Not Be Null");
            }

            _AppointmentStatusBusiness.Initialize(NewAppointmentStatus);
            if (await _AppointmentStatusBusiness.Save())
            {
                return Ok(_AppointmentStatusBusiness.AppointmentStatus.AppointmentStatusId);
            }

            return StatusCode(500, new { messege = "Error Adding New AppointmentStatus" });
        }

        [Authorize(Roles = "user")]
        [HttpPut("Update", Name = "UpdateAppointmentStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdateAppointmentStatus(AppointmentStatusDTO appointmentStatusDTO)
        {
            if (appointmentStatusDTO.AppointmentStatusId <= 0)
            {
                return BadRequest("The AppointmentStatus Id must be a valid number.");
            }

            _AppointmentStatusBusiness.Initialize(appointmentStatusDTO, SecurityHelper.EnMode.UpdateMode);
            if (await _AppointmentStatusBusiness.Save())
            {
                return Ok("AppointmentStatus Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating AppointmentStatus Data" });
        }

        [Authorize(Roles = "user")]
        [HttpDelete("Delete", Name = "DeleteAppointmentStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> DeleteAppointmentStatus(AppointmentStatusDTO appointmentStatusDTO)
        {
            if (appointmentStatusDTO.AppointmentStatusId <= 0)
            {
                return BadRequest("The AppointmentStatus Id must be a valid number.");
            }

            _AppointmentStatusBusiness.Initialize(appointmentStatusDTO, SecurityHelper.EnMode.DeleteMode);
            if (await _AppointmentStatusBusiness.Save())
            {
                return Ok("AppointmentStatus Info Deleted Succesfully");
            }

            return StatusCode(500, new { messege = "Error Deleting AppointmentStatus Data" });
        }

        [HttpGet("ById", Name = "GetAppointmentStatusById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentStatusDTO>> GetAppointmentStatusById(int AppointmentStatusId)
        {
            if (AppointmentStatusId <= 0)
            {
                return BadRequest("The AppointmentStatus Id must be a valid number.");
            }

            var appointmentStatus = await _AppointmentStatusBusiness.GetAppointmentStatusById(AppointmentStatusId);
            if (appointmentStatus != null)
            {
                return Ok(appointmentStatus);
            }

            return StatusCode(500, new { messege = "Error Getting AppointmentStatus Info" });
        }

        [HttpGet("ByFullName", Name = "GetAppointmentStatusByStatusName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentStatusDTO>> GetAppointmentStatusByStatusName(string StatusName)
        {
            if (string.IsNullOrWhiteSpace(StatusName))
            {
                return BadRequest("The AppointmentStatus Name must be a Provide.");
            }

            var appointmentStatus = await _AppointmentStatusBusiness.GetAppointmentStatusByStatusName(StatusName);
            if (appointmentStatus != null)
            {
                return Ok(appointmentStatus);
            }

            return StatusCode(500, new { messege = "Error Getting AppointmentStatus Info" });
        }

        [HttpGet("All", Name = "GetAllAppointmentStatuses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentStatusDTO>> GetAllAppointmentStatuses()
        {
            var appointmentStatus = await _AppointmentStatusBusiness.GetAllAppointmentStatuses();
            if (appointmentStatus != null)
            {
                return Ok(appointmentStatus);
            }

            return StatusCode(500, new { messege = "Error Getting AppointmentStatus Info" });
        }
    }
}
