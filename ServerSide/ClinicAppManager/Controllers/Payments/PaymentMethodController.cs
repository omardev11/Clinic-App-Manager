using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppManager.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodBusiness _PaymentMethodBusiness;

        public PaymentMethodController(IPaymentMethodBusiness paymentMethodBusiness)
        {
            _PaymentMethodBusiness = paymentMethodBusiness;
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [Authorize(Roles = "user")]
        [HttpPost(Name = "AddPaymentMethod")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> AddNewPaymentMethod(PaymentMethodDTO NewPaymentMethod)
        {
            if (NewPaymentMethod == null)
                return BadRequest("The Object Of The PaymentMethod Can Not Be Null");

            _PaymentMethodBusiness.Initialize(NewPaymentMethod);
            if (await _PaymentMethodBusiness.Save())
            {
                return Ok(_PaymentMethodBusiness.PaymentMethod.PaymentMethodId);
            }
            else
            {
                return StatusCode(500, new { message = "Error Adding New PaymentMethod" });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [Authorize(Roles = "user")]
        [HttpPut("Update", Name = "UpdatePaymentMethod")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdatePaymentMethod(
            int PaymentMethodId,
            [FromQuery] string? MethodName
        )
        {
            if (PaymentMethodId <= 0)
                return BadRequest("The PaymentMethod Id must be a valid number for authenication purpose.");

            PaymentMethodDTO PaymentMethodDTO = new PaymentMethodDTO()
            {
                PaymentMethodId = PaymentMethodId,
                PaymentMethodName = MethodName
            };

            _PaymentMethodBusiness.Initialize(PaymentMethodDTO, SecurityHelper.EnMode.UpdateMode);
            if (await _PaymentMethodBusiness.Save())
            {
                return Ok("PaymentMethod Info Updated Successfully");
            }
            else
            {
                return StatusCode(500, new { message = "Error Updating PaymentMethod Data " });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [Authorize(Roles = "user")]
        [HttpDelete("Delete", Name = "DeletePaymentMethod")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> DeletePaymentMethod(PaymentMethodDTO PaymentMethodDTO)
        {
            if (PaymentMethodDTO.PaymentMethodId <= 0)
                return BadRequest("The PaymentMethod Id must be a valid number.");

            _PaymentMethodBusiness.Initialize(PaymentMethodDTO, SecurityHelper.EnMode.DeleteMode);
            if (await _PaymentMethodBusiness.Save())
            {
                return Ok("PaymentMethod Info Deleted Successfully");
            }
            else
            {
                return StatusCode(500, new { message = "Error Deleting PaymentMethod Data" });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [HttpGet("ById", Name = "GetPaymentMethodById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentMethodDTO>> GetPaymentMethodById(int PaymentMethodId)
        {
            if (PaymentMethodId <= 0)
                return BadRequest("The PaymentMethod Id must be a valid number.");

            var PaymentMethod = await _PaymentMethodBusiness.GetPaymentMethodById(PaymentMethodId);
            if (PaymentMethod != null)
            {
                return Ok(PaymentMethod);
            }
            else
            {
                return StatusCode(500, new { message = "Error Getting PaymentMethod Info" });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [HttpGet("ByName", Name = "GetPaymentMethodByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentMethodDTO>> GetPaymentMethodByName(string MethodName)
        {
            if (string.IsNullOrWhiteSpace(MethodName))
                return BadRequest("The PaymentMethod Name must be provided.");

            var PaymentMethod = await _PaymentMethodBusiness.GetPaymentMethodByName(MethodName);
            if (PaymentMethod != null)
            {
                return Ok(PaymentMethod);
            }
            else
            {
                return StatusCode(500, new { message = "Error Getting PaymentMethod Info" });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        [HttpGet("All", Name = "GetAllPaymentMethods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentMethodDTO>>> GetAllPaymentMethods()
        {
            var PaymentMethods = await _PaymentMethodBusiness.GetAllPaymentMethods();
            if (PaymentMethods != null)
            {
                return Ok(PaymentMethods);
            }
            else
            {
                return StatusCode(500, new { message = "Error Getting PaymentMethod Info" });
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
    }
}

