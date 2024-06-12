

using AngularAuthAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Infrastructure.Repo
{
    public class UserRepo : IUser
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService emailService;
        private readonly IConfiguration config;

        public UserRepo(IConfiguration configuration, SignInManager<User> signInManager,
            UserManager<User> userManager, IEmailService emailService,
            IConfiguration config)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            this.emailService = emailService;
            this.config = config;
        }

        public async Task<LoginResponse> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null) return new LoginResponse(false, "Invalid username or password");
            if (user.EmailConfirmed == false) return new LoginResponse(false, "please confirm your email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return new LoginResponse(false, "Invalid username or password");
            var userDto = CreateApplicationUserDto(user);

            return new LoginResponse(true, $"{userDto.FirstName} {userDto.LastName} successfully logged in", userDto.JWT);


        }

        public async Task<GeneralResponse> Register(RegisterDto registerDto)
        {
            if (await CheckEmailExistAsync(registerDto.Email))
            {
                return new GeneralResponse(false, $"An existing account is using {registerDto.Email}, email address, please try with another email address");
            }
            var userToAdd = new User
            {
                FirstName = registerDto.FirstName.ToLower(),
                LastName = registerDto.LastName.ToLower(),
                UserName = registerDto.Email.ToLower(),
                Email = registerDto.Email.ToLower()
            };
            var result = await _userManager.CreateAsync(userToAdd, registerDto.Password);
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            if (!result.Succeeded) return new GeneralResponse(false, errors);
            try
            {
                if(await SendConfirmEmailAsync(userToAdd))
                {
                    return new GeneralResponse(true, "Your account has been created, please confirm the email");

                }
                return new GeneralResponse(false, "failed to send email for verification,Please contact admin");

            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, "failed to send email for verification,Please contact admin");

            }




        }

        public async Task<LoginResponse> RefreshToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new LoginResponse(false, "User not found");
            }
            var userDto = CreateApplicationUserDto(user);
            return new LoginResponse(true, "Token Refreshed", refreshToken: userDto.JWT);
        }
       
        #region Private Helper Methods
        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto(FirstName: user.FirstName, LastName: user.LastName, JWT: GenerateJWTToken(user));

        }
        private string GenerateJWTToken(User user)
        {

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim (ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.FirstName),
                new Claim(ClaimTypes.Surname,user.LastName),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication which should be atleast 512 bits"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:ExpiresInDays"])),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);
        }
        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }


        private async Task<bool> SendConfirmEmailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ConfirmationEmailPath"]}?token={token}&email={user.Email}";
            var body = $"<p>Hello: {user.FirstName}</p>" +
                "<p>Please confirm your email address by clicking on the following link</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{config["Email:ApplicationName"]}";
            var emailSend = new EmailSendDto(user.Email, "Confirm your email", EmailBody.EmailStringBody(user.FirstName,url, config["Email:ApplicationName"]));
            return await emailService.SendEmailAsync(emailSend);
        }


        #endregion
    }
}
