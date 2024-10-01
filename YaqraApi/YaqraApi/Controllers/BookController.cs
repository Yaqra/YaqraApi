using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Helpers;
using YaqraApi.Models.Enums;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IRecommendationService _recommendationService;
        private readonly ICommunityService _communityService;
        private readonly Mapper _mapper;

        public BookController(IBookService bookService, 
            IRecommendationService recommendationService, 
            ICommunityService communityService)
        {
            _bookService = bookService;
            _recommendationService = recommendationService;
            _communityService = communityService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("addBook")]
        public async Task<IActionResult> AddAsync(AddBookDto dto)
        {
            var result = await _bookService.AddAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result);

            return Created((string?)null, result);
        }
        
        [HttpGet("books")]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page)
        {
            var result = await _bookService.GetAll(page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(int bookId)
        {
            var result = await _bookService.GetByIdAsync(bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("title")]
        public async Task<IActionResult> GetByTitleAsync(string bookTitle)
        {
            var result = await _bookService.GetByTitle(bookTitle);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("titleAndId")]
        public async Task<IActionResult> GetAllTitlesAndIdsAsync([FromQuery] int page)
        {
            var result = await _bookService.GetAllTitlesAndIds(page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentAsync([FromQuery] int page)
        {
            var result = await _bookService.GetRecent(page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("image")]
        public async Task<IActionResult> UpdatePictureAsync(IFormFile image, [FromForm] int bookId)
        {
            if (bookId == 0)
                return BadRequest("author not found");

            var result = await _bookService.UpdateImageAsync(image, bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllAsync(
            IFormFile? image,
            [FromForm] int bookId,
            [FromForm] string? bookTitle,
            [FromForm] string? bookDescription,
            [FromForm] int? numberOfPages)
        {
            var dto = new BookWithoutImageDto { 
                Id = bookId,
                Title = bookTitle,
                NumberOfPages = numberOfPages,
                Description = bookDescription
            };
            if (dto == null)
                return BadRequest("something went wrong");

            var result = await _bookService.UpdateAllAsync(image, dto);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(BookIdDto dto)
        {
            var result = await _bookService.Delete(dto.BookId);

            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("bookPages")]
        public async Task<IActionResult> GetBooksPagesCount()
        {
            var result = await _bookService.GetBooksPagesCount();
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("addGenres")]
        public async Task<IActionResult> AddGenresToBook([FromForm] int bookId, [FromForm] HashSet<int> genreIds)
        {
            var result = await _bookService.AddGenresToBook(genreIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("removeGenres")]
        public async Task<IActionResult> RemoveGenresToBook([FromForm] int bookId, [FromForm] HashSet<int> genreIds)
        {
            var result = await _bookService.RemoveGenresFromBook(genreIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("addAuthors")]
        public async Task<IActionResult> AddAuthorsToBook([FromForm] int bookId, [FromForm] HashSet<int> authorIds)
        {
            var result = await _bookService.AddAuthorsToBook(authorIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("removeAuthors")]
        public async Task<IActionResult> RemoveAuthorsToBook([FromForm] int bookId, [FromForm] HashSet<int> authorIds)
        {
            var result = await _bookService.RemoveAuthorsFromBook(authorIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews(int bookId, int page, SortType sortType, ReviewsSortField sortField)
        {
            var result = await _bookService.GetReviews(bookId, page, sortType, sortField);
            if (result.Succeeded == false)
                return BadRequest(result);
            var LikedPosts = await _communityService.ArePostsLiked(result.Result.Data.Select(r => r.Id).ToList(), UserHelpers.GetUserId(User));
            foreach (var item in result.Result.Data)
            {
                if (LikedPosts.Contains(item.Id) == true)
                    item.IsLiked = true;
            }
            return Ok(result);
        }
        [HttpGet("find")]
        public async Task<IActionResult> FindBooks(decimal? MinimumRate, [FromQuery]HashSet<int>? AuthorIds, [FromQuery] HashSet<int>? GenreIds, int Page)
        {
            var dto = new BookFinderDto
            {
                MinimumRate = MinimumRate,
                AuthorIds = AuthorIds,
                GenreIds = GenreIds,
                Page = Page
            };
            var result = await _bookService.FindBooks(dto);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendedBooks()
        {
            var result = await _recommendationService.RecommendBooks(UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingBooks()
        {
            var result = await _bookService.GetTrendingBooks();
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingBooks([FromQuery] int page)
        {
            var result = await _bookService.GetUpcomingBooks(page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
