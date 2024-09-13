using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YaqraApi.DTOs.Genre;
using YaqraApi.Helpers;
using YaqraApi.Services;
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
        public async Task<IActionResult> GetAllAsync([FromQuery] int page)
        {
            var result = await _genreService.GetAllAsync(page);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("name")]
        public async Task<IActionResult> GetByNameAsync([FromQuery]string genreName)
        {
            var result = await _genreService.GetByNameAsync(genreName);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(int genreId)
        {
            var result = await _genreService.GetByIdAsync(genreId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("addGenre")]
        public async Task<IActionResult> AddAsync([FromQuery]string genreName)
        {
            var result = await _genreService.AddAsync(genreName);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Created((string?)null, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateGenreDto dto)
        {
            var result = await _genreService.UpdateAsync(dto.CurrentGenreId,dto.NewGenreName);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(GenreIdDto dto)
        {
            var result = await _genreService.DeleteAsync(dto.GenreId);
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("genrePages")]
        public async Task<IActionResult> GetBooksPagesCount()
        {
            var result = await _genreService.GetPagesCount();
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
