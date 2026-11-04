using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicAppManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientBusiness _PatientBusiness;
        private readonly IDoctorBusiness _DoctorBusiness;
        private readonly IAuthorizationService _authorizationService;

        public PatientController(
            IPatientBusiness PatientBusiness,
            IDoctorBusiness doctorBusiness,
            IAuthorizationService authorizationService)
        {
            _PatientBusiness = PatientBusiness;
            _DoctorBusiness = doctorBusiness;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "user,patient")]
        [HttpPost(Name = "AddPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AddNewPatient(PatientDTO NewPatient)
        {
            if (NewPatient == null)
            {
                return BadRequest("The Object Of The Patient Can Not Be Null");
            }

            string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(authUserId))
            {
                return Unauthorized();
            }

            string? userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "patient")
            {
                NewPatient.AspNetUserId = authUserId;

                if (await _PatientBusiness.CheckPatientExistByAspNetUserId(authUserId))
                {
                    return Conflict(new { error = "This patient already has a saved profile." });
                }
            }
            else if (string.IsNullOrWhiteSpace(NewPatient.AspNetUserId))
            {
                return BadRequest("AspNetUserId is required when a user creates a patient profile.");
            }

            _PatientBusiness.Initialize(NewPatient);
            if (await _PatientBusiness.Save())
            {
                return Ok("Success");
            }

            return StatusCode(500, new { messege = "Error Adding New Patient" });
        }

        [Authorize(Roles = "user,doctor")]
        [HttpPut("StaffUpdate", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdatePatient(
            int PatientId,
            [FromQuery] string? BloodType,
            [FromQuery] string? FirstName,
            [FromQuery] string? LastName,
            [FromQuery] string? Adress,
            [FromQuery] DateOnly? DateOfBirth,
            [FromQuery] char? Gender)
        {
            PatientDTO updatingPatient = new PatientDTO
            {
                PatientId = PatientId,
                BloodType = BloodType,
                Person = new PersonDTO
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Address = Adress,
                    DateOfBirth = DateOfBirth,
                    Gender = Gender
                }
            };

            if (PatientId <= 0)
            {
                return BadRequest("The Patient Id must be a valid number.");
            }

            _PatientBusiness.Initialize(updatingPatient, SecurityHelper.EnMode.UpdateMode);
            if (await _PatientBusiness.Save())
            {
                return Ok("Patient Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating Patient Data" });
        }

        [Authorize(Roles = "patient")]
        [HttpPut("Update", Name = "UpdatePatientInfoFromPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdatePatientInfoFromPatient(
            int PatientId,
            [FromQuery] string? Adress)
        {
            string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(authUserId))
            {
                return Unauthorized();
            }

            var existingPatient = await _PatientBusiness.GetPatientInfoByAspNetUserId(authUserId);
            if (existingPatient == null || existingPatient.PatientId != PatientId)
            {
                return Forbid();
            }

            PatientDTO updatingPatient = new PatientDTO
            {
                PatientId = PatientId,
                Person = new PersonDTO
                {
                    Address = Adress
                }
            };

            if (PatientId <= 0)
            {
                return BadRequest("The Patient Id must be a valid number.");
            }

            _PatientBusiness.Initialize(updatingPatient, SecurityHelper.EnMode.UpdateMode);
            if (await _PatientBusiness.Save())
            {
                return Ok("Patient Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating Patient Data" });
        }

        [Authorize(Roles = "user,doctor,patient")]
        [HttpGet("ById", Name = "GetPatientInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientDTO>> GetPatientInfoById(int PatientId)
        {
            if (PatientId <= 0)
            {
                return BadRequest("The Patient Id must be a valid number.");
            }

            var patient = await _PatientBusiness.GetPatientInfoById(PatientId);
            if (patient == null)
            {
                return StatusCode(500, new { messege = "Error Getting Patient Info" });
            }

            if (User.IsInRole("doctor"))
            {
                string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var doctor = await _DoctorBusiness.GetDoctorInfoByAspNetUserId(authUserId);
                if (doctor == null || !doctor.DoctorId.HasValue)
                {
                    return Forbid();
                }

                bool hasAccess = await _PatientBusiness.CheckIfThisPatientHasThisDoctor(
                    PatientId,
                    doctor.DoctorId.Value);
                if (!hasAccess)
                {
                    return Forbid();
                }
            }
            else
            {
                var authResult = await _authorizationService.AuthorizeAsync(User, patient.AspNetUserId, "OwnerOrAdminPolicy");
                if (!authResult.Succeeded)
                {
                    return Forbid();
                }
            }

            return Ok(patient);
        }

        [Authorize(Roles = "user,doctor")]
        [HttpGet("ByFullName", Name = "GetPatientInfoByFullName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientDTO>> GetPatientInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return BadRequest("The Patient Name must be a Provide.");
            }

            var patient = await _PatientBusiness.GetPatientInfoByFullName(FullName);
            
            if (patient != null)
            {
                if (User.IsInRole("doctor"))
                {
                    string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var doctor = await _DoctorBusiness.GetDoctorInfoByAspNetUserId(authUserId);
                    if (doctor == null || !doctor.DoctorId.HasValue)
                    {
                        return Forbid();
                    }

                    bool hasAccess = await _PatientBusiness.CheckIfThisPatientHasThisDoctor(
                        patient.PatientId.Value,
                        doctor.DoctorId.Value);
                    if (!hasAccess)
                    {
                        return Forbid();
                    }
                }
                return Ok(patient);
            }

            return StatusCode(500, new { messege = "Error Getting Patient Info" });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientDTO>> GetAllPatients()
        {
            var patient = await _PatientBusiness.GetAllPatients();

            if (patient != null)
            {
                return Ok(patient);
            }

            return StatusCode(500, new { messege = "Error Getting Patient Info" });
        }
    }
}
