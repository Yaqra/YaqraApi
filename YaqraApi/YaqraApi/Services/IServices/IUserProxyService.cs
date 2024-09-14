using YaqraApi.DTOs.User;
using YaqraApi.DTOs;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IUserProxyService
    {
        Task<GenericResultDto<UserDto>> GetUserAsync(string userId, string followerId);
        Task<GenericResultDto<ApplicationUser>> UpdateAllAsync(IFormFile? pic, IFormFile? cover, UserDto dto);
        Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId);
        Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId);

    }
}
