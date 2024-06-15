
using RealEstate.Application.DTOs.Response.Admin;

namespace RealEstate.Application.Contracts
{
    public interface IAdmin
    {
        Task<GetMembersResponse> GetMembers();
        Task<GeneralResponse> LockMember(string Id);

    }
}
