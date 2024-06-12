using RealEstate.Application.DTOs.Request;


namespace RealEstate.Application.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailSendDto emailSend);
    }
}
