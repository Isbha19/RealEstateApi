using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;


namespace RealEstate.Application.Contracts
{
    public interface IUser
    {
        Task<LoginResponse> Login(LoginDto loginDto);
        Task<GeneralResponse> Register(RegisterDto registerDto); 
    }
}
