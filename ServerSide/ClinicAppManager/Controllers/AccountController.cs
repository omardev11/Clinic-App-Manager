using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.DTO.Auth;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace ClinicAppManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger; 
        public AccountController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IDataBusiness dataBusiness,
            IConfiguration configuration,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dataBusiness = dataBusiness;
            this._logger = logger;
        }

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IDataBusiness _dataBusiness;

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(AppUserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser appUser = new()
            {
                Email = user.Email,
                UserName = user.Email
            };

            IdentityResult createUserResult = await _userManager.CreateAsync(appUser, user.Password);
            if (!createUserResult.Succeeded)
            {
                foreach (IdentityError error in createUserResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            if (!await _roleManager.RoleExistsAsync(user.Role))
            {
                IdentityResult createRoleResult = await _roleManager.CreateAsync(new IdentityRole(user.Role));
                if (!createRoleResult.Succeeded)
                {
                    foreach (IdentityError error in createRoleResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    await _userManager.DeleteAsync(appUser);
                    return BadRequest(ModelState);
                }
            }

            IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(appUser, user.Role);
            if (!addToRoleResult.Succeeded)
            {
                foreach (IdentityError error in addToRoleResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                await _userManager.DeleteAsync(appUser);
                return BadRequest(ModelState);
            }

            return Ok(new
            {
                message = "User registered successfully",
                authId = appUser.Id,
                email = appUser.Email,
                role = user.Role
            });
        }

        [HttpPost("Login")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Login(LoginDTO user)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!ModelState.IsValid)
            {
                // only log failures for login
                _logger.LogWarning("Login failed due to invalid model state. Email={Email}, IP={IP}", user.Email, ip);
                return BadRequest(ModelState);
            }

            AppUser? existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                _logger.LogWarning("Failed login attempt (email not found). Email={Email}, IP={IP}", user.Email, ip);
                return Unauthorized("Invalid credentials");

            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Failed login attempt (invalid password). Email={Email}, IP={IP}", user.Email, ip);

                return Unauthorized("Invalid credentials");
            }

            return Ok(await IssueingAccessToken(existingUser));

           
        }

        [Authorize]
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            AppUser? existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                return NotFound("User not found");
            }
            IdentityResult passwordChangeResult = await _userManager.ChangePasswordAsync(existingUser, model.CurrentPassword, model.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                foreach (IdentityError error in passwordChangeResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok("Password updated successfully");
        }

        [HttpPost("RefreshToken")]
        [EnableRateLimiting("AuthLimiter")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ApnetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ApnetUserId == null)
            { 
                return Unauthorized("Invalid Refresh Request");

            }

            var data = await _dataBusiness.GetDataByAspNetUserId(ApnetUserId);


            if (data.RevokedDate != null)
            {
                return Unauthorized("Refresh token has been revoked");
            }
            if (data.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                await _dataBusiness.DeleteRefreshTokenAfterGeneratingNewOne(ApnetUserId);
                return Unauthorized("Refresh Token has expired");
            }
            
            bool isRefreshTokenValid = SecurityHelper.VerifyPassword(refreshRequest.RefreshToken, data.RefreshToken);
            if (!isRefreshTokenValid)
                return Unauthorized("Invalid Refresh Token");

            return Ok(await IssueingAccessToken(await _userManager.FindByIdAsync(ApnetUserId)));

        }

       
        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ApnetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ApnetUserId == null)
                return Ok();

            var data = await _dataBusiness.GetDataByAspNetUserId(ApnetUserId);

            if (!SecurityHelper.VerifyPassword(logoutRequest.RefreshToken, data.RefreshToken))
                return Ok();
           
            data.RevokedDate = DateTime.UtcNow;
            _dataBusiness.Initialize(data, SecurityHelper.EnMode.UpdateMode);
            if (await _dataBusiness.Save())
                return Ok("Logged out successfully");
            
            return StatusCode(500, "Failed to log out");


        }

        private  static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
                

            }


        }

        private async Task<TokenResponse> IssueingAccessToken(AppUser existingUser)
        {
            var roles = await _userManager.GetRolesAsync(existingUser);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, existingUser.Email!),
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );
            var refreshToken = GenerateRefreshToken();

            _dataBusiness.Initialize(new DataDTO
            {
                AppUserId = existingUser.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                RevokedDate = null
            }, SecurityHelper.EnMode.AddNewMode);
            if (!await _dataBusiness.DeleteRefreshTokenAfterGeneratingNewOne(existingUser.Id))
                throw new Exception("Failed to delete old refresh token");
            if (!await _dataBusiness.Save())
                throw new Exception("Failed to save refresh token");

            return new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                AccessTokenExpiryTime = token.ValidTo
            };
        }
    }
}
