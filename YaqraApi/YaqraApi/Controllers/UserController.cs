using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Genre;
using YaqraApi.DTOs.ReadingGoal;
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
        private readonly Mapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }

        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllAsync(
            IFormFile? pic,
            IFormFile? cover,
            [FromForm] string? userName,
            [FromForm] string? userBio)
        {
            var userDto = new UserDto { Bio = userBio, UserId = UserHelpers.GetUserId(User), Username=userName };
            var result = await _userService.UpdateAllAsync(pic, cover, userDto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok("user updated successfully");
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
        public async Task<IActionResult> FollowUser(UserIdDto dto /*the user u want to follow*/)
        {
            var result = await _userService.FollowUserAsync(dto, UserHelpers.GetUserId(User));
            if(result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok($"{result.Result.Follower.UserName} followed {result.Result.Followed.UserName} successfully");
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetUserAsync(UserIdDto dto)
        {
            var result = await _userService.GetUserAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("followrs")]
        public async Task<IActionResult> GetFollowrsListAsync(UserIdDto dto)
        {
            var result = _userService.GetUserFollowersNames(dto.UserId);
            if(result.Succeeded==false)
                return BadRequest(result.ErrorMessage);
            else 
                return Ok(result.Result);
        }
        [HttpGet("followings")]
        public async Task<IActionResult> GetFollowingsListAsync(UserIdDto dto)
        {
            var result = _userService.GetUserFollowingsNames(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            else
                return Ok(result.Result);
        }
        [HttpPost("addFavGenres")]
        public async Task<IActionResult> AddFavouriteGenresAsync(List<GenreIdDto> genres)
         {
            var result = await _userService.AddFavouriteGenresAsync(genres, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("genresExcept")]
        public async Task<IActionResult> GetAllExceptUserGenresAsync()
        {
            var result = await _userService.GetAllGenresExceptUserGenresAsync(UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("favGenres")]
        public async Task<IActionResult> GetFavouriteGenresAsync()
        {
            var result = await _userService.GetFavouriteGenresAsync(UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete("favGenre")]
        public async Task<IActionResult> DeleteFavouriteGenreAsync(GenreIdDto genreId)
        {
            var result = await _userService.DeleteFavouriteGenreAsync(genreId, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpPost("addFavAuthors")]
        public async Task<IActionResult> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors)
        {
            var result = await _userService.AddFavouriteAuthorsAsync(authors, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("authorsExcept")]
        public async Task<IActionResult> GetAllExceptUserAuthorsAsync()
        {
            var result = await _userService.GetAllAuthorsExceptUserAuthorsAsync(UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("favAuthors")]
        public async Task<IActionResult> GetFavouriteAuthors()
        {
            var result = await _userService.GetFavouriteAuthorsAsync(UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete("favAuthor")]
        public async Task<IActionResult> DeleteFavouriteAuthorAsync(AuthorIdDto authorId)
        {
            var result = await _userService.DeleteFavouriteAuthorAsync(authorId, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPost("goal")]
        public async Task<IActionResult> AddReadingGoalAsync(AddReadingGoalDto addReadingGoalDto)
        {
            var dto = _mapper.Map<ReadingGoalDto>(addReadingGoalDto);
            dto.UserId = UserHelpers.GetUserId(User);

            var result = await _userService.AddReadingGoalAsync(dto, dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("allGoals")]
        public async Task<IActionResult> GetAllReadingGoalsAsync()
        {
            var result = await _userService.GetAllReadingGoalsAsync(UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete("goal")]
        public async Task<IActionResult> DeleteReadingGoalAsync([FromQuery]int goalId)
        {
            var result = await _userService.DeleteReadingGoalAsync(goalId, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("goal")]
        public async Task<IActionResult> UpdateReadingGoalAsync(UpdateReadingGoalDto dto)
        {
            var result = await _userService.UpdateReadingGoalAsync(dto, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

    }
}
