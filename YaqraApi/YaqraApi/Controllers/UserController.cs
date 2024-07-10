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
            if (await _userService.UpdateBioAsync(dto.NewBio, UserHelpers.GetUserId(User)) == false)
                return BadRequest("something went wrong");
            return Ok("bio updated successfully");
        }

        [HttpPut("username")]
        public async Task<IActionResult> UpdateUsername(UsernameUpdateDto dto)
        {
            if (await _userService.UpdateUsernameAsync(dto.Username, UserHelpers.GetUserId(User)) == false)
                return BadRequest("something went wrong");
            return Ok("username updated successfully");

        }
    }
}
