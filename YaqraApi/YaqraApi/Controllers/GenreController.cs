using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YaqraApi.DTOs.Genre;
using YaqraApi.Helpers;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly IUserService _userService;

        public GenreController(IGenreService genreService, IUserService userService)
        {
            _genreService = genreService;
            _userService = userService;
        }
        [HttpGet("genres")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _genreService.GetAllAsync();
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("name")]
        public async Task<IActionResult> GetByNameAsync([FromQuery]string genreName)
        {
            var result = await _genreService.GetByNameAsync(genreName);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(GenreIdDto genre)
        {
            var result = await _genreService.GetByIdAsync(genre.GenreId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPost("addGenre")]
        public async Task<IActionResult> AddAsync([FromQuery]string genreName)
        {
            var result = await _genreService.AddAsync(genreName);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateGenreDto dto)
        {
            var result = await _genreService.UpdateAsync(dto.CurrentGenreId,dto.NewGenreName);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(GenreIdDto dto)
        {
            var result = await _genreService.DeleteAsync(dto.GenreId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
    }
}
