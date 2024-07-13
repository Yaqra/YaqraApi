using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YaqraApi.DTOs.User;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }
        
        [HttpPut("bio")]
        public async Task<IActionResult> UpdateBio(BioUpdateDto dto)
        {
            var result = await _userService.UpdateBioAsync(dto.NewBio, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("bio updated successfully");
        }

        [HttpPut("username")]
        public async Task<IActionResult> UpdateUsername(UsernameUpdateDto dto)
        {
            var result = await _userService.UpdateUsernameAsync(dto.Username, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("username updated successfully");
        }
        [HttpPut("pass")]
        public async Task<IActionResult> UpdatePassword(PasswordUpdateDto dto)
        {
            var result = await _userService.UpdatePasswordAsync(dto, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("password updated successfully");
        }
        [HttpPut("profilePic")]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile pic)
        {
            var result = await _userService.UpdateProfilePictureAsync(pic, UserHelpers.GetUserId(User));
            if(result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("profile picture updated successfully");
        }
        [HttpPut("profileCover")]
        public async Task<IActionResult> UpdateProfileCover(IFormFile pic)
        {
            var result = await _userService.UpdateProfileCoverAsync(pic, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("profile picture updated successfully");
        }
        [HttpPost("follow")]
        public async Task<IActionResult> FollowUser(FollowUserDto dto)
        {
            var result = await _userService.FollowUserAsync(dto, UserHelpers.GetUserId(User));
            if(result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok($"{result.Result.Follower.UserName} followed {result.Result.Followed.UserName} successfully");
        }
    }
}
