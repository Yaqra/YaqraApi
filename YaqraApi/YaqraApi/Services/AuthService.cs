using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IConfiguration _configuration;

        public AuthService(IOptions<JwtConfig> jwt, 
            UserManager<ApplicationUser> userManager, 
            ApplicationContext context,
            IConfiguration configuration)
        {
            _jwt = jwt;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);

            if (user == null || await _userManager.CheckPasswordAsync(user, loginDto.Password) == false)
                return new AuthDto { Message = "username or password is incorrect" };

            var token = await CreateAccessTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var authDto = new AuthDto
            {
                IsAuthenticated = true,
                Username = user?.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive);
            if(activeRefreshToken != null)
            {
                authDto.RefreshToken = activeRefreshToken.Token;
                authDto.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authDto.RefreshToken = refreshToken.Token;
                authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return authDto;
        }
        private async Task<JwtSecurityToken> CreateAccessTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in userRoles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid",user.Id)
            }
            .Union(roleClaims);

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwt.Value.Issuer,
                    audience: _jwt.Value.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(_jwt.Value.DurationInDays),
                    signingCredentials: signingCredentials
                );
            return jwtSecurityToken;
        }
        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto, List<string> roles)
        {
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
                return new AuthDto { Message = "username already exists" };

            var user = new ApplicationUser { UserName = registerDto.Username, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded == false)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");
                return new AuthDto { Message= errors.ToString() };
            }

            foreach (var role in roles)
            {
                result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded == false)
                    return new AuthDto { Message = $"couldn't assign {role} to user" };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return new AuthDto
            {
                IsAuthenticated = true,
                Message = "Registered Successfully",
                Roles = userRoles.ToList(),
                Username = registerDto.Username,
                Email = user.Email
            };
            
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = RandomNumberGenerator.GetBytes(32);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("RefreshTokenDurationInDays")),
                CreatedOn = DateTime.UtcNow,
            };
        }
        public async Task<AuthDto> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken));
            if (user == null)
                return new AuthDto { Message = "invalid token" };

            var userRefreshToken = user.RefreshTokens.Single(r => r.Token == refreshToken);
            if (userRefreshToken.IsActive == false)
                return new AuthDto { Message = "inactive token" };

            userRefreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var accessToken = await CreateAccessTokenAsync(user);
            return new AuthDto
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                Username = user.UserName,
                Email = user.Email,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }
    }
}
