using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Service
{
    public class MainUser_Repo : IMainUser_Repo
    {
        private readonly UserManager<Users_App> userManager;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;

        public MainUser_Repo(UserManager<Users_App> _userManager, IConfiguration _configuration, IUnitOfWork unitOfWork)
        {
            userManager = _userManager;
            configuration = _configuration;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<User_appDTO>> Regisiter_User(Regisiter_DTO _DTO)
        {
            Users_App? user = await userManager.FindByNameAsync(_DTO.UserName);
            if (user != null)
                return Result<User_appDTO>.Fail(null, "UserName not available");

            Users_App? user2 = await userManager.FindByEmailAsync(_DTO.Email);
            if (user2 != null)
                return Result<User_appDTO>.Fail(null, "Email unavaliable");

            Users_App newUser = new Users_App()
            { UserName = _DTO.UserName, Email = _DTO.Email, PhoneNumber = _DTO.PhoneNumber, Created_At = DateTime.UtcNow };

            IdentityResult result = await userManager.CreateAsync(newUser, _DTO.Password);
            if (!result.Succeeded)
                return Result<User_appDTO>.Fail(null, $"User failed \n{result.Errors}");

            return Result<User_appDTO>.Success(new User_appDTO() { Name = _DTO.UserName, Email = _DTO.Email });
        }
        public async Task<Result<UserAuthResponse_DTO>> LogIn_User(LogIn_DTO _DTO)
        {
            var user = await userManager.FindByEmailAsync(_DTO.Email);
            if (user == null)
                return Result<UserAuthResponse_DTO>.Fail(null, "The email or password you entered is incorrect");

            var check = await userManager.CheckPasswordAsync(user, _DTO.Password);
            if (!check)
                return Result<UserAuthResponse_DTO>.Fail(null, "The email or password you entered is incorrect");

            string token = await GenerateToken(user);

            return Result<UserAuthResponse_DTO>.Success(
                new UserAuthResponse_DTO()
                {
                    Email = user.Email,
                    Name = user.UserName,
                    Token = token
                });
        }

        private async Task<string> GenerateToken(Users_App user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));


            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            SigningCredentials credentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudiance"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Users_App> GetCurrentUser(ClaimsPrincipal claims)
        {
            var id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<string> GetUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return (user != null) ? user.Id : null;
        }
    }


}
