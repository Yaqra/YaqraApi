using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly Mapper _mapper;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("addAuthor")]
        public async Task<IActionResult> AddAsync(
            IFormFile? picture,
            [FromForm] string authorName, 
            [FromForm] string? authorBio)
        {
            var dto = new AddAuthorDto { Bio = authorBio, Name = authorName };
            var result = await _authorService.AddAsync(picture, _mapper.Map<AuthorDto>(dto));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);

            return Created((string?)null, result.Result);
        }
        
        [HttpGet("authors")]
        public async Task<IActionResult> GetAllAsync([FromQuery]int page)
        {
            var result = await _authorService.GetAll(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        
        [HttpGet("authorPages")]
        public async Task<IActionResult> GetAuthorsPagesCount()
        {
            var result = await _authorService.GetAuthorsPagesCount();
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        
        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(AuthorIdDto idDto)
        {
            var result = await _authorService.GetByIdAsync(idDto.AuthorId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpGet("name")]
        public async Task<IActionResult> GetByNameAsync([FromQuery] string authorName, [FromQuery] int page)
        {
            var result = await _authorService.GetByName(authorName, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpGet("nameAndId")]
        public async Task<IActionResult> GetAllNamesAndIdsAsync([FromQuery] int page)
        {
            var result = await _authorService.GetAllNamesAndIds(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }

        [HttpGet("books")]
        public async Task<IActionResult> GetAuthorBooksAsync([FromQuery] int authorId, [FromQuery] int page)
        {
            var result = await _authorService.GetAuthorBooks(authorId, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("picture")]
        public async Task<IActionResult> UpdatePictureAsync(IFormFile picture, [FromForm]int authorId)
        {
            if(authorId == 0)
                return BadRequest("author not found");

            var result = await _authorService.UpdatePictureAsync(picture,authorId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllAsync(
            IFormFile? picture,
            [FromForm]int authorId,
            [FromForm]string? authorName, 
            [FromForm]string? authorBio)
        {
            var dto = new AuthorWithoutPicDto { Id =  authorId, Name = authorName, Bio = authorBio }; 
            if (dto == null)
                return BadRequest("something went wrong");

            var result = await _authorService.UpdateAllAsync(picture, dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(AuthorIdDto dto)
        {
            var result = await _authorService.Delete(dto.AuthorId);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
    }
}
