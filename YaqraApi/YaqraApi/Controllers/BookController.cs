using AutoMapper;
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
        private readonly Mapper _mapper;

        public BookController(IBookService bookService, IRecommendationService recommendationService)
        {
            _bookService = bookService;
            _recommendationService = recommendationService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        [HttpPost("addBook")]
        public async Task<IActionResult> AddAsync(AddBookDto dto)
        {
            var result = await _bookService.AddAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);

            return Created((string?)null, result.Result);
        }
        
        [HttpGet("books")]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page)
        {
            var result = await _bookService.GetAll(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(BookIdDto idDto)
        {
            var result = await _bookService.GetByIdAsync(idDto.BookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("title")]
        public async Task<IActionResult> GetByTitleAsync(string bookTitle, [FromQuery] int page)
        {
            var result = await _bookService.GetByTitle(bookTitle, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("titleAndId")]
        public async Task<IActionResult> GetAllTitlesAndIdsAsync([FromQuery] int page)
        {
            var result = await _bookService.GetAllTitlesAndIds(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentAsync([FromQuery] int page)
        {
            var result = await _bookService.GetRecent(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("image")]
        public async Task<IActionResult> UpdatePictureAsync(IFormFile image, [FromForm] int bookId)
        {
            if (bookId == 0)
                return BadRequest("author not found");

            var result = await _bookService.UpdateImageAsync(image, bookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
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
                Description = bookDescription,
            };
            if (dto == null)
                return BadRequest("something went wrong");

            var result = await _bookService.UpdateAllAsync(image, dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(BookIdDto dto)
        {
            var result = await _bookService.Delete(dto.BookId);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("bookPages")]
        public async Task<IActionResult> GetBooksPagesCount()
        {
            var result = await _bookService.GetBooksPagesCount();
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpPut("addGenres")]
        public async Task<IActionResult> AddGenresToBook([FromForm] int bookId, [FromForm] List<int> genreIds)
        {
            var result = await _bookService.AddGenresToBook(genreIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("removeGenres")]
        public async Task<IActionResult> RemoveGenresToBook([FromForm] int bookId, [FromForm] List<int> genreIds)
        {
            var result = await _bookService.RemoveGenresFromBook(genreIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("addAuthors")]
        public async Task<IActionResult> AddAuthorsToBook([FromForm] int bookId, [FromForm] List<int> authorIds)
        {
            var result = await _bookService.AddAuthorsToBook(authorIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("removeAuthors")]
        public async Task<IActionResult> RemoveAuthorsToBook([FromForm] int bookId, [FromForm] List<int> authorIds)
        {
            var result = await _bookService.RemoveAuthorsFromBook(authorIds, bookId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews([FromForm] int bookId, [FromForm] int page, [FromForm] SortType sortType, [FromForm] ReviewsSortField sortField)
        {
            var result = await _bookService.GetReviews(bookId, page, sortType, sortField);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("find")]
        public async Task<IActionResult> FindBooks(BookFinderDto dto)
        {
            var result = await _bookService.FindBooks(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendedBooks()
        {
            var result = await _recommendationService.RecommendBooks(UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingBooks()
        {
            var result = await _bookService.GetTrendingBooks();
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingBooks([FromQuery] int page)
        {
            var result = await _bookService.GetUpcomingBooks(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
    }
}
