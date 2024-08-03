using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Community;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly Mapper _mapper;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();

        }
        [HttpGet("review")]
        public async Task<IActionResult> GetReviewAsync([FromQuery] int reviewId)
        {
            var result = await _communityService.GetReviewAsync(reviewId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("review")]
        public async Task<IActionResult> AddReviewAsync(AddReviewDto dto)
        {
            var review = _mapper.Map<ReviewDto>(dto);
            var result = await _communityService.AddReviewAsync(review);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("review")]
        public async Task<IActionResult> UpdateReviewAsync(ReviewDto dto)
        {
            var result = await _communityService.UpdateReviewAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int postId)
        {
            var result = await _communityService.Delete(postId);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
    }
}
