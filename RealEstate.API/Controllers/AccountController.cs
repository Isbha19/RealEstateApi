using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;

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

        public async Task<ActionResult<LoginResponse>> Register(RegisterDto registerDto)
        {
            var result = await _user.Register(registerDto);
            return Ok(result);
        }


    }
}
