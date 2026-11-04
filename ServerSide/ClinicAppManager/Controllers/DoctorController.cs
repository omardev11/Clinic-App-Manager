using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicAppManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorBusiness _DoctorBusiness;
        private readonly IAuthorizationService _authorizationService;

        public DoctorController(IDoctorBusiness DoctorBusiness, IAuthorizationService authorizationService)
        {
            _DoctorBusiness = DoctorBusiness;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "user")]
        [HttpPost(Name = "AddDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AddNewDoctor(DoctorDTO NewDoctor)
        {
            if (NewDoctor == null)
            {
                return BadRequest("The Object Of The Doctor Can Not Be Null");
            }

            if (string.IsNullOrWhiteSpace(NewDoctor.AspNetUserId))
            {
                return BadRequest("AspNetUserId is required.");
            }

            if (await _DoctorBusiness.CheckDoctorExistByAspNetUserId(NewDoctor.AspNetUserId))
            {
                return Conflict(new { error = "This doctor already has saved data." });
            }

            _DoctorBusiness.Initialize(NewDoctor);
            if (await _DoctorBusiness.Save())
            {
                return Ok(_DoctorBusiness.Doctor.DoctorId);
            }

            return StatusCode(500, new { messege = "Error Adding New Doctor" });
        }

        [Authorize(Roles = "user")]
        [HttpPut("StaffUpdate", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdateDoctor(
            int DoctorId,
            [FromQuery] string? Specialization,
            [FromQuery] string? PhoneNumber,
            [FromQuery] bool? IsActive,
            [FromQuery] string? FirstName,
            [FromQuery] string? LastName,
            [FromQuery] string? Adress,
            [FromQuery] DateOnly? DateOfBirth,
            [FromQuery] char? Gender)
        {
            DoctorDTO updatingDoctor = new DoctorDTO
            {
                DoctorId = DoctorId,
                Specialization = Specialization,
                PhoneNumber = PhoneNumber,
                IsActive = IsActive,
                Person = new PersonDTO
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Address = Adress,
                    DateOfBirth = DateOfBirth,
                    Gender = Gender
                }
            };

            if (DoctorId <= 0)
            {
                return BadRequest("The Doctor Id must be a valid number.");
            }

            _DoctorBusiness.Initialize(updatingDoctor, SecurityHelper.EnMode.UpdateMode);
            if (await _DoctorBusiness.Save())
            {
                return Ok("Doctor Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating Doctor Data" });
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("Update", Name = "UpdateDoctorInfoFromDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdateDoctorInfoFromDoctor(
            int DoctorId,
            [FromQuery] string? PhoneNumber,
            [FromQuery] string? Adress)
        {
            string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(authUserId))
            {
                return Unauthorized();
            }

            var existingDoctor = await _DoctorBusiness.GetDoctorInfoByAspNetUserId(authUserId);
            if (existingDoctor == null || existingDoctor.DoctorId != DoctorId)
            {
                return Forbid();
            }

            DoctorDTO updatingDoctor = new DoctorDTO
            {
                DoctorId = DoctorId,
                PhoneNumber = PhoneNumber,
                Person = new PersonDTO
                {
                    Address = Adress
                }
            };

            if (DoctorId <= 0)
            {
                return BadRequest("The Doctor Id must be a valid number.");
            }

            _DoctorBusiness.Initialize(updatingDoctor, SecurityHelper.EnMode.UpdateMode);
            if (await _DoctorBusiness.Save())
            {
                return Ok("Doctor Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating Doctor Data" });
        }

        [Authorize(Roles = "user,doctor")]
        [HttpGet("ById", Name = "GetDoctorInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorDTO>> GetDoctorInfoById(int DoctorId)
        {
            if (DoctorId <= 0)
            {
                return BadRequest("The Doctor Id must be a valid number.");
            }

            var doctor = await _DoctorBusiness.GetDoctorInfoById(DoctorId);
            if (doctor == null)
            {
                return StatusCode(500, new { messege = "Error Getting Doctor Info" });
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, doctor.AspNetUserId, "OwnerOrAdminPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(doctor);
        }

        [Authorize(Roles = "user")]
        [HttpGet("ByFullName", Name = "GetDoctorInfoByFullName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorDTO>> GetDoctorInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return BadRequest("The Doctor Name must be a Provide.");
            }

            var doctor = await _DoctorBusiness.GetDoctorInfoByFullName(FullName);
            if (doctor != null)
            {
                return Ok(doctor);
            }

            return StatusCode(500, new { messege = "Error Getting Doctor Info" });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorDTO>> GetAllDoctors()
        {
            var doctor = await _DoctorBusiness.GetAllDoctors();

            if (doctor != null)
            {
                return Ok(doctor);
            }

            return StatusCode(500, new { messege = "Error Getting Doctor Info" });
        }
    }
}
