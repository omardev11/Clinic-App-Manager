using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.Appointments;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicAppManager.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentBusiness _PaymentBusiness;
        private readonly IAppointmentBusiness _AppointmentBusiness;
        private readonly IPatientBusiness _PatientBusiness;
        private readonly IDoctorBusiness _DoctorBusiness;

        public PaymentController(
            IPaymentBusiness paymentBusiness,
            IAppointmentBusiness appointmentBusiness,
            IPatientBusiness patientBusiness,
            IDoctorBusiness doctorBusiness)
        {
            _PaymentBusiness = paymentBusiness;
            _AppointmentBusiness = appointmentBusiness;
            _PatientBusiness = patientBusiness;
            _DoctorBusiness = doctorBusiness;
        }

        [Authorize(Roles = "user")]
        [HttpPost(Name = "AddPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewPayment(PaymentDTO newPayment)
        {
            if (newPayment == null)
                return BadRequest("The Object Of The Payment Can Not Be Null");
          
            _PaymentBusiness.Initialize(newPayment, SecurityHelper.EnMode.AddNewMode);
            if (await _PaymentBusiness.Save())
            {
                return Ok(_PaymentBusiness.Payment.PaymentId);
            }

            return StatusCode(500, new { message = "Error Adding New Payment" });
        }

        [Authorize(Roles = "user")]
        [HttpPut(Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdatePayment(
            int PaymentId,
            [FromQuery] decimal? AmountPaid,
            [FromQuery] int? PaymentMethodId,
            [FromQuery] DateTime? paymentDate,
            [FromQuery] string? AdditionalNotes,
            [FromQuery] int? AppointmentId)
        {
            PaymentDTO payment = new()
            {
                PaymentId = PaymentId,
                AmountPaid = AmountPaid,
                PaymentMethodId = PaymentMethodId,
                PaymentDate = paymentDate,
                AdditionalNotes = AdditionalNotes,
                AppointmentId = AppointmentId
            };

            if (payment.PaymentId == null || payment.PaymentId <= 0)
                return BadRequest("The Payment Id must be valid.");

            _PaymentBusiness.Initialize(payment, SecurityHelper.EnMode.UpdateMode);
            if (await _PaymentBusiness.Save())
            {
                return Ok("Payment Info Updated Successfully");
            }

            return StatusCode(500, new { message = "Error Updating Payment Data" });
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByPaymentId", Name = "GetPaymentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DetailPaymentDTO>> GetPaymentById(int paymentId)
        {
            if (paymentId <= 0)
                return BadRequest("The Payment Id must be valid.");

            var payment = await _PaymentBusiness.GetPaymentById(paymentId);
            if (payment == null)
                return StatusCode(500, new { message = "Error Getting Payment Info" });

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
       .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier), payment.PaymentDTO.AppointmentId.Value);
            if (!hasAccess)
            {
                return Forbid();
            }
           
            return Ok(payment);
        }

        [Authorize(Roles = "patient,doctor,user")]
        [HttpGet("ByAppointmentId", Name = "GetPaymentByAppointmentId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DetailPaymentDTO>> GetPaymentByAppointmentId(int appointmentId)
        {
            if (appointmentId <= 0)
                return BadRequest("The Appointment Id must be valid.");

            bool hasAccess = await _AppointmentBusiness.HasAccessToAppointmentAsync(User.FindAll(ClaimTypes.Role)
        .Select(Claim => Claim.Value), User.FindFirstValue(ClaimTypes.NameIdentifier),appointmentId);
            if (!hasAccess)
            {
                return Forbid();
            }

            var payment = await _PaymentBusiness.GetPaymentByAppointmentId(appointmentId);
            if (payment != null)
                return Ok(payment);

            return StatusCode(500, new { message = "Error Getting Payment Info" });
        }

        [Authorize(Roles = "user")]
        [HttpGet("All", Name = "GetAllPayments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DetailPaymentDTO>>> GetAllPayments()
        {
            var list = await _PaymentBusiness.GetAllPayments();
            return Ok(list);
        }
    }
}
