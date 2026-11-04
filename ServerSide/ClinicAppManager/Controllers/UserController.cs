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
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _UserBusiness;

        public UserController(IUserBusiness UserBusiness)
        {
            _UserBusiness = UserBusiness;
        }

        [Authorize(Roles = "admin")]
        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AddNewUser(UserDTO NewUser)
        {
            if (NewUser == null)
            {
                return BadRequest("The Object Of The User Can Not Be Null");
            }

            if (string.IsNullOrWhiteSpace(NewUser.AspNetUserId))
            {
                return BadRequest("AspNetUserId is required.");
            }

            if (await _UserBusiness.CheckUserExistByAspNetUserId(NewUser.AspNetUserId))
            {
                return Conflict(new { error = "This user already has saved data." });
            }

            if (!string.IsNullOrWhiteSpace(NewUser.UserName) &&
                await _UserBusiness.CheckUserNameExist(NewUser.UserName))
            {
                return Conflict(new { error = "This username is already registered." });
            }

            _UserBusiness.Initialize(NewUser);
            if (await _UserBusiness.Save())
            {
                return Ok(_UserBusiness.User.UserId);
            }

            return StatusCode(500, new { messege = "Error Adding New User" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("AdminUpdate", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdateUser(
            int UserId,
            [FromQuery] string? UserName,
            [FromQuery] bool? IsActive,
            [FromQuery] string? FirstName,
            [FromQuery] string? LastName,
            [FromQuery] string? Adress,
            [FromQuery] DateOnly? DateOfBirth,
            [FromQuery] char? Gender)
        {
            UserDTO updatingUser = new UserDTO
            {
                UserId = UserId,
                UserName = UserName,
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

            if (UserId <= 0)
            {
                return BadRequest("The User Id must be a valid number.");
            }

            if (!string.IsNullOrWhiteSpace(UserName) &&
                await _UserBusiness.CheckUserNameExist(UserName))
            {
                return Conflict(new { error = "This username is already registered." });
            }

            _UserBusiness.Initialize(updatingUser, SecurityHelper.EnMode.UpdateMode);
            if (await _UserBusiness.Save())
            {
                return Ok("User Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating User Data" });
        }

        [Authorize(Roles = "user")]
        [HttpPut("Update", Name = "UpdateUserInfoFromUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<string>> UpdateUserInfoFromUser(
            int UserId,
            [FromQuery] string? Adress)
        {
            string? authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(authUserId))
            {
                return Unauthorized();
            }

            var existingUser = await _UserBusiness.GetUserInfoByAspNetUserId(authUserId);
            if (existingUser == null || existingUser.UserId != UserId)
            {
                return Forbid();
            }

            UserDTO updatingUser = new UserDTO
            {
                UserId = UserId,
                Person = new PersonDTO
                {
                    Address = Adress
                }
            };

            if (UserId <= 0)
            {
                return BadRequest("The User Id must be a valid number.");
            }

            _UserBusiness.Initialize(updatingUser, SecurityHelper.EnMode.UpdateMode);
            if (await _UserBusiness.Save())
            {
                return Ok("User Info Updated Succesfully");
            }

            return StatusCode(500, new { messege = "Error Updating User Data" });
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("ById", Name = "GetUserInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDTO>> GetUserInfoById(int UserId)
        {
            if (UserId <= 0)
            {
                return BadRequest("The User Id must be a valid number.");
            }

            var user = await _UserBusiness.GetUserInfoById(UserId);
            if (user == null)
            {
                return StatusCode(500, new { messege = "Error Getting User Info" });
            }

            var authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "user" && user.AspNetUserId != authUserId)
            {
                return Forbid();
            }
 
            return Ok(user);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("ByFullName", Name = "GetUserInfoByFullName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        
        public async Task<ActionResult<UserDTO>> GetUserInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return BadRequest("The User Name Must be Provided.");
            }

            var user = await _UserBusiness.GetUserInfoByFullName(FullName);
            if (user != null)
            {
                return Ok(user);
            }

            return StatusCode(500, new { messege = "Error Getting User Info" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("ByUserName", Name = "GetUserInfoByUserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDTO>> GetUserInfoByUserName(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                return BadRequest("The User Name must be a Provide.");
            }

            var user = await _UserBusiness.GetUserInfoByUserName(UserName);
            if (user != null)
            {
                return Ok(user);
            }

            return StatusCode(500, new { messege = "Error Getting User Info" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetAllUsers()
        {
            var user = await _UserBusiness.GetAllUsers();

            if (user != null)
            {
                return Ok(user);
            }

            return StatusCode(500, new { messege = "Error Getting User Info" });
        }
    }
}
