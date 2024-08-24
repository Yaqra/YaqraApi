using Microsoft.AspNetCore.Identity;
using YaqraApi.DTOs;
using YaqraApi.DTOs.User;
using YaqraApi.Models;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class UserProxyService : IUserProxyService
    {
        private readonly IUserService _userService;
        //                        userId   details
        private static Dictionary<string, UserDto> Users = new();

        public UserProxyService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GenericResultDto<UserDto>> GetUserAsync(string userId)
        {
            if(Users.ContainsKey(userId))
                return new GenericResultDto<UserDto> { Succeeded = true, Result = Users[userId] };
            
            var userResult = await _userService.GetUserAsync(userId);
            if(userResult.Succeeded)
                Users.Add(userId, userResult.Result);
            return userResult;
        }

        public async Task<GenericResultDto<ApplicationUser>> UpdateAllAsync(IFormFile? pic, IFormFile? cover, UserDto dto)
        {
            Users.Remove(dto.UserId);
            return await _userService.UpdateAllAsync(pic, cover, dto);
        }

        public async Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId)
        {
            Users.Remove(userId);
            return await _userService.UpdateProfilePictureAsync(pic, userId);
        }

        public async Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId)
        {
            Users.Remove(userId);
            return await _userService.UpdateProfileCoverAsync(pic, userId);
        }

        public async Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId)
        {
            Users.Remove(userId);
            Users.Remove(dto.UserId);
            return await _userService.FollowUserAsync(dto, userId);
        }
    }
}
