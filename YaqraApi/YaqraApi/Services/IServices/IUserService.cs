using System.Security.Claims;

namespace YaqraApi.Services.IServices
{
    public interface IUserService
    {
        Task<bool> UpdateBioAsync(string bio, string userId);
        Task<bool> UpdateUsernameAsync(string username, string userId);
    }
}
