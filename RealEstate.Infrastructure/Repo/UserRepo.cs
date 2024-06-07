

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs.Account;
using RealEstate.Application.DTOs.Request.Account;
using RealEstate.Application.DTOs.Response.Account;
using RealEstate.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Infrastructure.Repo
{
    public class UserRepo:IUser
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserRepo(IConfiguration configuration,SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
          _configuration = configuration;
          _signInManager = signInManager;
          _userManager = userManager;
        }

        public async Task<LoginResponse> Login(LoginDto loginDto)
        {
            var user=await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null) return new LoginResponse(false, "Invalid username or password");
            if (user.EmailConfirmed == false) return new LoginResponse(false, "please confirm your email");

            var result=await _signInManager.CheckPasswordSignInAsync(user,loginDto.Password,false);
            if (!result.Succeeded) return new LoginResponse(false, "Invalid username or password");
            var userDto = CreateApplicationUserDto(user);

            return new LoginResponse(true,$"{userDto.FirstName} {userDto.LastName} successfully logged in",userDto.JWT); 


        }

        public async Task<GeneralResponse> Register(RegisterDto registerDto)
        {
            if(await CheckEmailExistAsync(registerDto.Email))
            {
                return new GeneralResponse(false, $"An existing account is using {registerDto.Email}, email address, please try with another email address");
            }
            var userToAdd = new User
            {
                FirstName = registerDto.FirstName.ToLower(),
                LastName = registerDto.LastName.ToLower(),
                UserName = registerDto.Email.ToLower(),
                Email = registerDto.Email.ToLower(),
                EmailConfirmed = true
            };
            var result=await _userManager.CreateAsync(userToAdd,registerDto.Password);
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            if (!result.Succeeded) return new GeneralResponse(false, errors);

            return new GeneralResponse(true, "Your account has been created, you can login");



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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
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
            return await _userManager.Users.AnyAsync(x=>x.Email == email.ToLower());
        }

    
        #endregion
    }
}
