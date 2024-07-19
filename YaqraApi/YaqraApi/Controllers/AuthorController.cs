using AutoMapper;
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
        [HttpPost("addAuthor")]
        public async Task<IActionResult> AddAsync(IFormFile? picture, [FromForm]string authorDetails)
        {
            var dto = JsonConvert.DeserializeObject<AddAuthorDto>(authorDetails);

            var result = await _authorService.AddAsync(picture, _mapper.Map<AuthorDto>(dto));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);

            return Created((string?)null, result.Result);
        }
        [HttpGet("authors")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _authorService.GetAll();
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
        public async Task<IActionResult> GetByNameAsync(string authorName)
        {
            var result = await _authorService.GetByName(authorName);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("nameAndId")]
        public async Task<IActionResult> GetAllNamesAndIdsAsync()
        {
            var result = await _authorService.GetAllNamesAndIds();
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
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
        [HttpPut("all")]
        public async Task<IActionResult> UpdateAllAsync(IFormFile picture, [FromForm] string authorDetails)
        {
            var dto = JsonConvert.DeserializeObject<AuthorWithoutPicDto>(authorDetails);
            if (dto == null)
                return BadRequest("something went wrong");

            var result = await _authorService.UpdateAllAsync(picture, dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPut("allWithoutPicture")]
        public async Task<IActionResult> UpdateWithoutPicAsync(AuthorWithoutPicDto dto)
        {
            var result = await _authorService.UpdateWithoutPicAsync(dto);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);

        }
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
