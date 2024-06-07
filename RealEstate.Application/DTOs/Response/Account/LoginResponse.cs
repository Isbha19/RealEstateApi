

namespace RealEstate.Application.DTOs.Response.Account
{
    public record LoginResponse(bool Flag=false,string Message=null,string Token=null,string refreshToken=null);
}
