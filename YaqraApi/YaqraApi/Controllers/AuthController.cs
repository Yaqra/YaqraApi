using Microsoft.AspNetCore.Mvc;
using YaqraApi.DTOs.Auth;
using YaqraApi.Helpers;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto, new List<string> { Roles.User });
            if(result.IsAuthenticated == false)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdminAsync(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto, new List<string> { Roles.User, Roles.Admin });
            if (result.IsAuthenticated == false)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if(result.IsAuthenticated == false)
                return BadRequest(result.Message);

            SetRefreshTokenInCookies(result.RefreshToken,result.RefreshTokenExpiration);
            return Ok(result);
        }
        private void SetRefreshTokenInCookies(string refreshToken, DateTime expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiration.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken",refreshToken, cookieOptions);
        }
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshAccessTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshAccessTokenAsync(refreshToken);

            if (result.IsAuthenticated == false)
                return BadRequest(result.Message);

            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("revokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshTokenAsync(RevokeRefreshTokenDto dto)
        {
            var token = dto.RefreshToken;
            if (string.IsNullOrEmpty(token))
                return BadRequest("token is required");

            var result = await _authService.RevokeRefreshTokenAsync(token);
            if (result == false)
                return NotFound("token is invalid");

            return Ok();
        }

    }
}
