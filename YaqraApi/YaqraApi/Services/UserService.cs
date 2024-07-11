using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
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

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
    }
}
