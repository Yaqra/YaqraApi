using System.Security.Claims;

namespace YaqraApi.Services.IServices
{
    public interface IUserService
    {
        Task<bool> EditBioAsync(string bio, string userId);
    }
}
