using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;
using RealEstate.Infrastructure.Repo;
using System.Security.Claims;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUser _user;

        public AccountController(IUser user)
        {
            this._user = user;
        }
        [HttpPost("login")]

        public async Task<ActionResult<LoginResponse>> Login(LoginDto loginDto)
        {
            var result=await _user.Login(loginDto);
            return Ok(result);
        }
        [HttpPost("register")]

        public async Task<ActionResult<GeneralResponse>> Register(RegisterDto registerDto)
        {
            var result = await _user.Register(registerDto);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<GeneralResponse>> RefreshUserToken()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found");
            }

            var response = await _user.RefreshToken(email);
            if (!response.Flag)
            {
                return BadRequest(response.Message);
            }

            return Ok(new { token = response.refreshToken, message = response.Message });
        }

    }
}
