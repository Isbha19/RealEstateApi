using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;

namespace RealEstate.Application.Contracts
{
    public interface IUser
    {
        Task<LoginResponse> Login(LoginDto loginDto);
        Task<GeneralResponse> Register(RegisterDto registerDto);
        Task<LoginResponse> RefreshToken(string email);
        Task<GeneralResponse> ConfirmEmail(ConfirmEmailDto confirmEmailDto);
        Task<GeneralResponse> ResendEmailConfirmation(string email);
        Task<GeneralResponse> ForgotUsernameorPassword(string email);
        Task<GeneralResponse> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<GeneralResponse<UserDto>> RegisterWithThirdParty(RegisterWithExternalDto model);
    }
}
