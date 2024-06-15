using Microsoft.AspNetCore.Identity;
using RealEstate.Application.DTOs.Request.Admin;
using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs.Response.Admin;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Extensions;


namespace RealEstate.Infrastructure.Repo
{
    public class AdminRepo:IAdmin
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminRepo(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
   
        public async Task<GetMembersResponse> GetMembers()
        {
            var members = await userManager.Users
                 .Where(x => x.UserName != Constant.AdminUserName)
                 //projection
                 .Select(member => new MemberViewDto
                 {
                     Id = member.Id,
                     UserName = member.UserName,
                     FirstName = member.FirstName,
                     DateCreated = member.DateCreated,
                     IsLocked = userManager.IsLockedOutAsync(member).GetAwaiter().GetResult(),
                     Roles = userManager.GetRolesAsync(member).GetAwaiter().GetResult()
                 }).ToListAsync();

            var response = new GetMembersResponse
            {
                Members = members,
                Message = "Members retrieved successfully",
                Success = true
            };

            return response;
        }

        public async Task<GeneralResponse> LockMember(string Id)
        {
            var user=await userManager.FindByIdAsync(Id);
            if (user == null) return new GeneralResponse(false,"user not found");
            if (IsAdmin(Id))
            {
                return new GeneralResponse(false, Constant.SuperAdminChangeNotAllowed);
            }
            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return new GeneralResponse(true);
        }

        #region private helper methods
        private bool IsAdmin(string userId)
        {
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(Constant.AdminUserName);
        }
        #endregion
    }
}
