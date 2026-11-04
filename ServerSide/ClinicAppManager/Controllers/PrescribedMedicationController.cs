using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.Appointments;
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
    public class PrescribedMedicationController : ControllerBase
    {
        private readonly IPrescribedMedicationBusiness _PrescribedMedicationBusiness;
        private readonly IMedicalRecordBusiness _MedicalRecordBusiness;
        private readonly IAppointmentBusiness _AppointmentBusiness;
        private readonly IPatientBusiness _PatientBusiness;
        private readonly IDoctorBusiness _DoctorBusiness;

        public PrescribedMedicationController(
            IPrescribedMedicationBusiness prescribedMedicationBusiness,
            IMedicalRecordBusiness medicalRecordBusiness,
            IAppointmentBusiness appointmentBusiness,
            IPatientBusiness patientBusiness,
            IDoctorBusiness doctorBusiness)
        {
            _PrescribedMedicationBusiness = prescribedMedicationBusiness;
            _MedicalRecordBusiness = medicalRecordBusiness;
            _AppointmentBusiness = appointmentBusiness;
            _PatientBusiness = patientBusiness;
            _DoctorBusiness = doctorBusiness;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost(Name = "AddPrescribedMedication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewPrescribedMedication(PrescribedMedicationDTO newPrescribedMedication)
        {
            if (newPrescribedMedication == null)
                return BadRequest("The Object Of The Prescribed Medication Can Not Be Null");

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(newPrescribedMedication.RecordId.Value);

            if (medicalRecord == null)
             {
                return NotFound(new { message = "The Medical Record Not Found" });
            }
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            _PrescribedMedicationBusiness.Initialize(newPrescribedMedication, SecurityHelper.EnMode.AddNewMode);
            if (await _PrescribedMedicationBusiness.Save())
            {
                return Ok(_PrescribedMedicationBusiness.PrescribedMedication.PrescriptionId);
            }

            return StatusCode(500, new { message = "Error Adding New Prescribed Medication" });
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("Update", Name = "UpdatePrescribedMedication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdatePrescribedMedication(
            int PrescriptionId,
            [FromQuery] string? MedicatoinName,
            [FromQuery] string? Dosage,
            [FromQuery] string? Frequency,
            [FromQuery] DateOnly? StartDate,
            [FromQuery] DateOnly? EndDate,
            [FromQuery] string? SpecialInstructions,
            [FromQuery] int? RecordId)
        {
            PrescribedMedicationDTO prescribedMedication = new()
            {
                PrescriptionId = PrescriptionId,
                MedicatoinName = MedicatoinName,
                Dosage = Dosage,
                Frequency = Frequency,
                StartDate = StartDate,
                EndDate = EndDate,
                SpecialInstructions = SpecialInstructions,
                RecordId = RecordId
            };

            if (prescribedMedication.PrescriptionId == null || prescribedMedication.PrescriptionId <= 0)
                return BadRequest("The Prescription Id must be valid.");

            var existingMedication = await _PrescribedMedicationBusiness.GetPrescribedMedicationById(PrescriptionId);
            if (existingMedication == null)
            {
                return NotFound(new { message = "Prescribed Medication not found." });
            }

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(existingMedication.RecordId.Value);

            if (medicalRecord == null)
            {
                return NotFound(new { message = "The Medical Record Not Found" });
            }
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            _PrescribedMedicationBusiness.Initialize(prescribedMedication, SecurityHelper.EnMode.UpdateMode);
            if (await _PrescribedMedicationBusiness.Save())
            {
                return Ok("Prescribed Medication Info Updated Successfully");
            }

            return StatusCode(500, new { message = "Error Updating Prescribed Medication Data" });
        }

        [Authorize(Roles = "doctor")]
        [HttpDelete("Delete", Name = "DeletePrescribedMedication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> DeletePrescribedMedication(PrescribedMedicationDTO prescribedMedicationDTO)
        {
            if (prescribedMedicationDTO.PrescriptionId == null || prescribedMedicationDTO.PrescriptionId <= 0)
                return BadRequest("The Prescription Id must be valid.");

            var existingMedication = await _PrescribedMedicationBusiness.GetPrescribedMedicationById(
                prescribedMedicationDTO.PrescriptionId.Value);
            if (existingMedication == null || !existingMedication.RecordId.HasValue)
            {
                return NotFound(new { message = "Prescribed Medication not found." });
            }

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(existingMedication.RecordId.Value);

            if (medicalRecord == null)
            {
                return NotFound(new { message = "The Medical Record Not Found" });
            }
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            _PrescribedMedicationBusiness.Initialize(prescribedMedicationDTO, SecurityHelper.EnMode.DeleteMode);
            if (await _PrescribedMedicationBusiness.Save())
            {
                return Ok("Prescribed Medication deleted successfully.");
            }

            return StatusCode(500, new { message = "Error deleting Prescribed Medication data." });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByPrescriptionId", Name = "GetPrescribedMedicationById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrescribedMedicationDTO>> GetPrescribedMedicationById(int prescriptionId)
        {
            if (prescriptionId <= 0)
                return BadRequest("The Prescription Id must be valid.");

            var medication = await _PrescribedMedicationBusiness.GetPrescribedMedicationById(prescriptionId);
            if (medication == null)
                return StatusCode(500, new { message = "Error Getting Prescribed Medication Info" });

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(medication.RecordId.Value);

            if (medicalRecord == null)
            {
                return NotFound(new { message = "The Medical Record Not Found" });
            }
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            return Ok(medication);
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByRecordId", Name = "GetPrescribedMedicationByRecordId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrescribedMedicationDTO>> GetPrescribedMedicationByRecordId(int recordId)
        {
            if (recordId <= 0)
                return BadRequest("The Record Id must be valid.");

            var medicalRecord = await _MedicalRecordBusiness.GetMedicalRecordById(recordId);

            if (medicalRecord == null)
            {
                return NotFound(new { message = "The Medical Record Not Found" });
            }
            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), medicalRecord.AppoinmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }

            var medication = await _PrescribedMedicationBusiness.GetPrescribedMedicationByRecordId(recordId);
            if (medication != null)
                return Ok(medication);

            return StatusCode(500, new { message = "Error Getting Prescribed Medication Info" });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllPrescribedMedications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PrescribedMedicationDTO>>> GetAllPrescribedMedications()
        {
            var list = await _PrescribedMedicationBusiness.GetAllPrescribedMedications();
            return Ok(list);
        }
    }
}
