using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;
using YaqraApi.DTOs;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IOptions<JwtConfig> _jwt;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        public AuthService(IOptions<JwtConfig> jwt, UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _jwt = jwt;
            _userManager = userManager;
            _context = context;
        }
        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto, List<string> roles)
        {
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
                return new AuthDto { Message = "username already exists" };
            
            var user = new ApplicationUser { UserName = registerDto.Username };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded == false)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error}, ");
                return new AuthDto { Message= errors.ToString() };
            }

            foreach (var role in roles)
            {
                result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded == false)
                    return new AuthDto { Message = $"couldn't assign {role} to user" };
            }
            
            return new AuthDto
            {
                IsAuthenticated = true,
                Message = "Registered Successfully",
                Roles = roles,
                Username = registerDto.Username,
            };
            
        }
    }
}
