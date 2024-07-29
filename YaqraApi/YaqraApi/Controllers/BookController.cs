using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly Mapper _mapper;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
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
                Description = bookDescription};
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
    }
}
