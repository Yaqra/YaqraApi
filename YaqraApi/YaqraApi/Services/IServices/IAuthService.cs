using YaqraApi.DTOs.Auth;

namespace YaqraApi.Services.IServices
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto registerDto, List<string> roles);
        Task<AuthDto> LoginAsync(LoginDto loginDto);
        Task<AuthDto> RefreshAccessTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}
