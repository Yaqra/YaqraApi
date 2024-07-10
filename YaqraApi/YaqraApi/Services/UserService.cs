using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public async Task<bool> UpdateBioAsync(string bio, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) 
                return false;

            user.Bio = bio;
            await _userManager.UpdateAsync(user);
            return true;
        }
        public async Task<bool> UpdateUsernameAsync(string username, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) 
                return false;
            await _userManager.SetUserNameAsync(user, username);
            return true;
        }
    }
}
