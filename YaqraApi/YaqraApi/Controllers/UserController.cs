
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
        public async Task<IActionResult> GetFollowrsListAsync(UserIdDto dto, [FromQuery]int page)
        {
            var result = _userService.GetUserFollowersNames(dto.UserId, page);
            if(result.Succeeded==false)
                return BadRequest(result.ErrorMessage);
            else 
                return Ok(result.Result);
        }
        [HttpGet("followings")]
        public async Task<IActionResult> GetFollowingsListAsync(UserIdDto dto, [FromQuery] int page)
        {
            var result = _userService.GetUserFollowingsNames(dto.UserId, page);
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
        public async Task<IActionResult> GetAllExceptUserGenresAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllGenresExceptUserGenresAsync(UserHelpers.GetUserId(User), page);
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
        public async Task<IActionResult> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors,[FromQuery] int page)
        {
            var result = await _userService.AddFavouriteAuthorsAsync(authors, UserHelpers.GetUserId(User), page);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("authorsExcept")]
        public async Task<IActionResult> GetAllExceptUserAuthorsAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllAuthorsExceptUserAuthorsAsync(UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("favAuthors")]
        public async Task<IActionResult> GetFavouriteAuthors([FromQuery] int page)
        {
            var result = await _userService.GetFavouriteAuthorsAsync(UserHelpers.GetUserId(User),page);

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
        public async Task<IActionResult> GetAllReadingGoalsAsync([FromQuery] int page)
        {
            var result = await _userService.GetAllReadingGoalsAsync(UserHelpers.GetUserId(User), page);
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
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("getBooks")]
        public async Task<IActionResult> GetBooksAsync([FromQuery] int? status, [FromQuery] int page)
        {
            var result = await _userService.GetBooksAsync(status, UserHelpers.GetUserId(User), page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpPut("updateBook")]
        public async Task<IActionResult> UpdateBookStatusAsync([FromQuery] int? status, [FromQuery] int bookId)
        {
            var result = await _userService.UpdateBookStatusAsync(bookId, status, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete("deleteBook")]
        public async Task<IActionResult> DeleteBookAsync([FromQuery] int bookId)
        {
            var result = await _userService.DeleteBookAsync(bookId, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("userFollowersPages")]
        public async Task<IActionResult> GetUserFollowersPagesCount(UserIdDto dto)
        {
            var result = _userService.GetUserFollowersPagesCount(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("userFollowingsPages")]
        public async Task<IActionResult> GetUserFollowingsPagesCount(UserIdDto dto)
        {
            var result = _userService.GetUserFollowingsPagesCount(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("genresExceptUserGenresPages")]
        public async Task<IActionResult> GetGenresExceptUserGenresPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetGenresExceptUserGenresPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("favouriteAuthorsPages")]
        public async Task<IActionResult> GetFavouriteAuthorsPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetFavouriteAuthorsPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("authorsExceptfavouriteAuthorsPages")]
        public async Task<IActionResult> GetAuthorsExceptfavouriteAuthorsPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetFavouriteAuthorsExceptUserPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("readingGoalPages")]
        public async Task<IActionResult> GetReadingGoalPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetReadingGoalsPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("bookCollectionPages")]
        public async Task<IActionResult> GetBookCollectionPagesCount(UserIdDto dto)
        {
            var result = await _userService.GetBooksPagesCountAsync(dto.UserId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
    }
}
