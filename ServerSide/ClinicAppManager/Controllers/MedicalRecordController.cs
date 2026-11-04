using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.Appointments;
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
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordBusiness _MedicalRecordBusiness;
        private readonly IAppointmentBusiness _AppointmentBusiness;
        private readonly IPatientBusiness _PatientBusiness;
        private readonly IDoctorBusiness _DoctorBusiness;

        public MedicalRecordController(
            IMedicalRecordBusiness medicalRecordBusiness,
            IAppointmentBusiness appointmentBusiness,
            IPatientBusiness patientBusiness,
            IDoctorBusiness doctorBusiness)
        {
            _MedicalRecordBusiness = medicalRecordBusiness;
            _AppointmentBusiness = appointmentBusiness;
            _PatientBusiness = patientBusiness;
            _DoctorBusiness = doctorBusiness;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost(Name = "AddMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewMedicalRecord(MedicalRecordDTO newMedicalRecord)
        {
            if (newMedicalRecord == null)
                return BadRequest("The object of MedicalRecord cannot be null.");
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), newMedicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            _MedicalRecordBusiness.Initialize(newMedicalRecord);
            if (await _MedicalRecordBusiness.Save())
            {
                return Ok(_MedicalRecordBusiness.MedicalRecord.RecordId);
            }

            return StatusCode(500, new { message = "Error adding new MedicalRecord." });
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("Update", Name = "UpdateMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdateMedicalRecord(
            int RecordId,
            [FromQuery] string? VisitDescription,
            [FromQuery] string? Diagnosis,
            [FromQuery] string? AdditionalNotes,
            [FromQuery] int? AppoinmentId)
        {
            if (RecordId <= 0)
                return BadRequest("The MedicalRecord Id must be a valid number.");

            var existingRecord = await _MedicalRecordBusiness.GetMedicalRecordById(RecordId);
            if (existingRecord == null)
            {
                return NotFound(new { message = "MedicalRecord not found." });
            }

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
       .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), existingRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

          
            var medicalRecordDTO = new MedicalRecordDTO()
            {
                RecordId = RecordId,
                VisitDescription = VisitDescription,
                Diagnosis = Diagnosis,
                AdditionalNotes = AdditionalNotes,
                AppoinmentId = AppoinmentId
            };

            _MedicalRecordBusiness.Initialize(medicalRecordDTO, SecurityHelper.EnMode.UpdateMode);

            if (await _MedicalRecordBusiness.Save())
            {
                return Ok("MedicalRecord information updated successfully.");
            }

            return StatusCode(500, new { message = "Error updating MedicalRecord data." });
        }

        [Authorize(Roles = "doctor")]
        [HttpDelete("Delete", Name = "DeleteMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> DeleteMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {
            if (medicalRecordDTO.RecordId == null || medicalRecordDTO.RecordId <= 0)
                return BadRequest("The MedicalRecord ID must be a valid number.");

            var existingRecord = await _MedicalRecordBusiness.GetMedicalRecordById(medicalRecordDTO.RecordId.Value);
            if (existingRecord == null || !existingRecord.AppoinmentId.HasValue)
            {
                return NotFound(new { message = "MedicalRecord not found." });
            }

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), existingRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            _MedicalRecordBusiness.Initialize(medicalRecordDTO, SecurityHelper.EnMode.DeleteMode);
            if (await _MedicalRecordBusiness.Save())
            {
                return Ok("MedicalRecord deleted successfully.");
            }

            return StatusCode(500, new { message = "Error deleting MedicalRecord data." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ById", Name = "GetMedicalRecordById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecordById(int recordId)
        {
            if (recordId <= 0)
                return BadRequest("The MedicalRecord ID must be a valid number.");

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(recordId);
            if (medicalRecord == null)
                return StatusCode(500, new { message = "Error retrieving MedicalRecord information." });

            if (!medicalRecord.AppoinmentId.HasValue)
            {
                return Forbid();
            }

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
                            .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);

            if (!hasAccess)
            {
                return Forbid();
            }
          

            return Ok(medicalRecord);
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByAppointmentId", Name = "GetMedicalRecordsByAppointmentId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecordsByAppointmentId(int AppointmentId)
        {
            if (AppointmentId <= 0)
                return BadRequest("The MedicalRecord ID must be a valid number.");
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
                            .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), AppointmentId);
            if (!hasAccess)
                return Forbid();

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordsByAppointmentId(AppointmentId);
           
            if (medicalRecord != null)
                return Ok(medicalRecord);

            return StatusCode(500, new { message = "Error retrieving MedicalRecord information." });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllMedicalRecords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetAllMedicalRecords()
        {
            var records = await _MedicalRecordBusiness.GetAllMedicalRecords();

            if (records != null)
                return Ok(records);

            return StatusCode(500, new { message = "Error retrieving MedicalRecords." });
        }
    }
}
