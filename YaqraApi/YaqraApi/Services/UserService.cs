using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.User;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mapper _mapper;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateBioAsync(string bio, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return new GenericResultDto<ApplicationUser>() { Succeeded = false, ErrorMessage = "user not found" };

            user.Bio = bio;
            var identityResult = await _userManager.UpdateAsync(user);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result=user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateUsernameAsync(string username, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) 
                return new GenericResultDto<ApplicationUser>() {Succeeded = false, ErrorMessage = "user not found" };
            var identityResult = await _userManager.SetUserNameAsync(user, username);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser> 
                { 
                    Succeeded = false, 
                    ErrorMessage = UserHelpers.GetErrors(identityResult) 
                };

            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdatePasswordAsync(PasswordUpdateDto dto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };
            
            var identityResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser> 
                { 
                    Succeeded = false, 
                    ErrorMessage = UserHelpers.GetErrors(identityResult) 
                };
            
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };
           
            var oldPicPath = user.ProfilePicture;

            var createPic = Task.Run(async () =>
            {
                var picName = Path.GetFileName(pic.FileName);
                var picExtension = Path.GetExtension(picName);
                var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";

                var picPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfilePictures", picWithGuid);

                using(var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    user.ProfilePicture = picPath;
                }
            });
            var deleteOldPic = Task.Run(() =>
            {
                if(string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };
           
            var oldPicPath = user.ProfileCover;

            var createPic = Task.Run(async () =>
            {
                var picName = Path.GetFileName(pic.FileName);
                var picExtension = Path.GetExtension(picName);
                var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";

                var picPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileCovers", picWithGuid);

                using(var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    user.ProfileCover = picPath;
                }
            });
            var deleteOldPic = Task.Run(() =>
            {
                if(string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<UserFollowerDto> { Succeeded = false, ErrorMessage = "user not found" };
            
            var followedUser = await _userManager.FindByIdAsync(dto.UserId);
            if (followedUser == null)
                return new GenericResultDto<UserFollowerDto> { Succeeded = false, ErrorMessage = "the user you want to follow not found" };

            user.Followings = new List<ApplicationUser> { followedUser };

            var identityResult = await _userManager.UpdateAsync(user);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<UserFollowerDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<UserFollowerDto> { Succeeded = true, Result = new UserFollowerDto {Follower= user, Followed= followedUser} };
        }
        public async Task<GenericResultDto<UserDto>> GetUserAsync(string userId)
        {
            var dto = await _userManager.Users.Select(u => new
            {
                Id = u.Id,
                Username = u.UserName,
                Bio = u.Bio,
                ProfilePicture = u.ProfilePicture,
                ProfileCover = u.ProfileCover,
                FollowersCount = u.Followers.Count(),
                FollowingsCount = u.Followings.Count(),
            }).SingleOrDefaultAsync(x => x.Id == userId);

            if (dto == null)
                return new GenericResultDto<UserDto> { Succeeded = false, ErrorMessage = "user not found" };

            return new GenericResultDto<UserDto>
            {
                Succeeded = true,
                Result = new UserDto
                {
                    UserId = dto.Id,
                    Username = dto.Username,
                    Bio = dto?.Bio,
                    ProfilePicture = dto?.ProfilePicture,
                    ProfileCover = dto?.ProfileCover,
                    FollowersCount = dto.FollowersCount,
                    FollowingsCount = dto.FollowingsCount,
                }
            };
            
        }
        public GenericResultDto<List<UsernameAndId>> GetUserFollowersNames(string userId)
        {
            var followersList = from user in _userManager.Users
                                where user.Id == userId
                                from follower in user.Followers
                                select new { follower.Id, follower.UserName }; 

            var followersDto = new List<UsernameAndId>();
            foreach (var follower in followersList)
                followersDto.Add(new UsernameAndId { UserId = follower.Id, Username = follower.UserName });

            return new GenericResultDto<List<UsernameAndId>> { Succeeded= true, Result = followersDto };
        
        }
        public GenericResultDto<List<UsernameAndId>> GetUserFollowingsNames(string userId)
        {
            var followingsList = from user in _userManager.Users
                                where user.Id == userId
                                from following in user.Followings
                                select new { following.Id, following.UserName };

            var followingDto = new List<UsernameAndId>();
            foreach (var following in followingsList)
                followingDto.Add(new UsernameAndId { UserId = following.Id, Username = following.UserName });

            return new GenericResultDto<List<UsernameAndId>> { Succeeded = true, Result = followingDto };
        }
    }
}
