
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Genre;
using YaqraApi.DTOs.ReadingGoal;
using YaqraApi.DTOs.User;
using YaqraApi.DTOs.UserBookWithStatus;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IUserProxyService _userProxyService;
        private readonly Mapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, 
            IUserService userService,
            IUserProxyService userProxyService)
        {
            _userManager = userManager;
            _userService = userService;
            _userProxyService = userProxyService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        [Authorize]
        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllAsync(
            IFormFile? pic,
            IFormFile? cover,
            [FromForm] string? userName,
            [FromForm] string? userBio)
        {
            var userDto = new UserDto { Bio = userBio, UserId = UserHelpers.GetUserId(User), Username=userName };
            var result = await _userProxyService.UpdateAllAsync(pic, cover, userDto);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok("user updated successfully");
        }
        [Authorize]
        [HttpPut("pass")]
        public async Task<IActionResult> UpdatePassword(PasswordUpdateDto dto)
        {
            var result = await _userService.UpdatePasswordAsync(dto, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok("password updated successfully");
        }
        [Authorize]
        [HttpPut("profilePic")]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile pic)
        {
            var result = await _userProxyService.UpdateProfilePictureAsync(pic, UserHelpers.GetUserId(User));
            if(result.Succeeded == false)
                return BadRequest(result);
            return Ok("profile picture updated successfully");
        }
        [Authorize]
        [HttpPut("profileCover")]
        public async Task<IActionResult> UpdateProfileCover(IFormFile pic)
        {
            var result = await _userProxyService.UpdateProfileCoverAsync(pic, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok("profile picture updated successfully");
        }
        [Authorize]
        [HttpPost("follow")]
        public async Task<IActionResult> FollowUser(UserIdDto dto /*the user u want to follow*/)
        {
            var result = await _userProxyService.FollowUserAsync(dto, UserHelpers.GetUserId(User));
            if(result.Succeeded == false)
                return BadRequest(result);
            return Ok(new GenericResultDto<string> { Succeeded = true, Result = $"{result.Result.Follower.UserName} followed {result.Result.Followed.UserName} successfully" });
        }
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserAsync(string userId)
        {
            var result = await _userProxyService.GetUserAsync(userId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowrsListAsync(string userId, int page)
        {
            var result = _userService.GetUserFollowersNames(userId, page);
            if(result.Succeeded==false)
                return BadRequest(result);
            else 
                return Ok(result);
        }
        [Authorize]
        [HttpGet("followings")]
        public async Task<IActionResult> GetFollowingsListAsync(string userId, int page)
        {
            var result = _userService.GetUserFollowingsNames(userId, page);
            if (result.Succeeded == false)
                return BadRequest(result);
            else
                return Ok(result);
        }
        [Authorize]
        [HttpPost("addFavGenres")]
        public async Task<IActionResult> AddFavouriteGenresAsync(List<GenreIdDto> genres)
         {
            var result = await _userService.AddFavouriteGenresAsync(genres, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("genresExcept")]
        public async Task<IActionResult> GetAllExceptUserGenresAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllGenresExceptUserGenresAsync(UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("favGenres")]
        public async Task<IActionResult> GetFavouriteGenresAsync()
        {
            var result = await _userService.GetFavouriteGenresAsync(UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("favGenre")]
        public async Task<IActionResult> DeleteFavouriteGenreAsync(GenreIdDto genreId)
        {
            var result = await _userService.DeleteFavouriteGenreAsync(genreId, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("addFavAuthors")]
        public async Task<IActionResult> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors,[FromQuery] int page)
        {
            var result = await _userService.AddFavouriteAuthorsAsync(authors, UserHelpers.GetUserId(User), page);

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("authorsExcept")]
        public async Task<IActionResult> GetAllExceptUserAuthorsAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllAuthorsExceptUserAuthorsAsync(UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("favAuthors")]
        public async Task<IActionResult> GetFavouriteAuthors([FromQuery] int page)
        {
            var result = await _userService.GetFavouriteAuthorsAsync(UserHelpers.GetUserId(User),page);

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("favAuthor")]
        public async Task<IActionResult> DeleteFavouriteAuthorAsync(AuthorIdDto authorId)
        {
            var result = await _userService.DeleteFavouriteAuthorAsync(authorId, UserHelpers.GetUserId(User));

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("goal")]
        public async Task<IActionResult> AddReadingGoalAsync(AddReadingGoalDto addReadingGoalDto)
        {
            var dto = _mapper.Map<ReadingGoalDto>(addReadingGoalDto);
            dto.UserId = UserHelpers.GetUserId(User);

            var result = await _userService.AddReadingGoalAsync(dto, dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("allGoals")]
        public async Task<IActionResult> GetAllReadingGoalsAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllReadingGoalsAsync(UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("goal")]
        public async Task<IActionResult> DeleteReadingGoalAsync([FromQuery]int goalId)
        {
            var result = await _userService.DeleteReadingGoalAsync(goalId, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("goal")]
        public async Task<IActionResult> UpdateReadingGoalAsync(UpdateReadingGoalDto dto)
        {
            var result = await _userService.UpdateReadingGoalAsync(dto, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("addBook")]
        public async Task<IActionResult> AddBookToCollectionAsync(AddUserBookWithStatusDto dto)
        {
            var userBookDto = new UserBookWithStatusDto
            {
                UserId = UserHelpers.GetUserId(User),
                Status = dto.Status,
                BookId = dto.BookId,
                AddedDate = dto.AddedDate
            };

            var result = await _userService.AddBookToCollectionAsync(userBookDto, userBookDto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("getBooks")]
        public async Task<IActionResult> GetBooksAsync([FromQuery] int? status, [FromQuery] int page)
        {
            var result = await _userService.GetBooksAsync(status, UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("updateBook")]
        public async Task<IActionResult> UpdateBookStatusAsync([FromQuery] int? status, [FromQuery] int bookId)
        {
            var result = await _userService.UpdateBookStatusAsync(bookId, status, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("deleteBook")]
        public async Task<IActionResult> DeleteBookAsync([FromQuery] int bookId)
        {
            var result = await _userService.DeleteBookAsync(bookId, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("userFollowersPages")]
        public async Task<IActionResult> GetUserFollowersPagesCount(UserIdDto dto)
        {
            var result = _userService.GetUserFollowersPagesCount(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("userFollowingsPages")]
        public async Task<IActionResult> GetUserFollowingsPagesCount(UserIdDto dto)
        {
            var result = _userService.GetUserFollowingsPagesCount(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("genresExceptUserGenresPages")]
        public async Task<IActionResult> GetGenresExceptUserGenresPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetGenresExceptUserGenresPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("favouriteAuthorsPages")]
        public async Task<IActionResult> GetFavouriteAuthorsPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetFavouriteAuthorsPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("authorsExceptfavouriteAuthorsPages")]
        public async Task<IActionResult> GetAuthorsExceptfavouriteAuthorsPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetFavouriteAuthorsExceptUserPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("readingGoalPages")]
        public async Task<IActionResult> GetReadingGoalPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetReadingGoalsPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("bookCollectionPages")]
        public async Task<IActionResult> GetBookCollectionPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetBooksPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
