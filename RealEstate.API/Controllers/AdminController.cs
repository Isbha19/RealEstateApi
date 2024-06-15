using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;


namespace RealEstate.API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin admin;

        public AdminController(IAdmin admin)
        {
            this.admin = admin;
        }
        [HttpGet("get-members")]
        public async Task<IActionResult> GetMembers()
        {
            var result = await admin.GetMembers();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }
        [HttpPut("lock-member/{Id}")]
        public async Task<IActionResult> LockMember(string Id)
        {
            var result = await admin.LockMember(Id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
