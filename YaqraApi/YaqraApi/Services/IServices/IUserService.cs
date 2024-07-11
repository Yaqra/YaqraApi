using System.Security.Claims;
using YaqraApi.DTOs;
using YaqraApi.DTOs.User;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IUserService
    {
        Task<GenericResultDto<ApplicationUser>> UpdateBioAsync(string bio, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateUsernameAsync(string username, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdatePasswordAsync(PasswordUpdateDto dto, string userId);
    }
}
