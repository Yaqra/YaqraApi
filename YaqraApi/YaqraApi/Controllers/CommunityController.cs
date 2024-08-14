using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Community;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
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
            return Ok(result.Result);
        }
        [HttpGet("allReviews")]
        public async Task<IActionResult> GetAllReviewsAsync([FromQuery] int page)
        {
            var result = await _communityService.GetAllReviewsAsync(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("allPlaylists")]
        public async Task<IActionResult> GetAllPlaylistsAsync([FromQuery] int page)
        {
            var result = await _communityService.GetAllPlaylistsAsync(page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("playlist")]
        public async Task<IActionResult> GetPlaylistAsync([FromQuery] int playlistId)
        {
            var result = await _communityService.GetPlaylistAsync(playlistId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("discussion")]
        public async Task<IActionResult> GetDiscussionAsync([FromQuery] int discussionId)
        {
            var result = await _communityService.GetDiscussionAsync(discussionId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPost("review")]
        public async Task<IActionResult> AddReviewAsync(AddReviewDto dto)
        {
            //var review = _mapper.Map<ReviewDto>(dto);
            var result = await _communityService.AddReviewAsync(dto, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPost("playlist")]
        public async Task<IActionResult> AddPlaylistAsync([FromForm] AddPlaylistDto dto)
        {
            var result = await _communityService.AddPlaylistAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPost("discussion")]
        public async Task<IActionResult> AddDiscussionAsync([FromForm] AddDiscussionArticleNewsDto dto)
        {
            var result = await _communityService.AddDiscussionAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("review")]
        public async Task<IActionResult> UpdateReviewAsync(UpdateReviewDto dto)
        {
            var result = await _communityService.UpdateReviewAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("playlist")]
        public async Task<IActionResult> UpdatePlaylistAsync(UpdatePlaylistDto dto)
        {
            var result = await _communityService.UpdatePlaylistAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("like")]
        public async Task<IActionResult> LikeAsync([FromQuery] int postId)
        {
            await _communityService.LikeAsync(postId);
            return NoContent();
        }
        [HttpPut("discussion")]
        public async Task<IActionResult> UpdateDiscussionAsync(UpdateDiscussionArticleNewsDto dto)
        {
            var result = await _communityService.UpdateDiscussionAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("addBooksToPlaylist")]
        public async Task<IActionResult> AddBooksToPlaylistAsync([FromForm]int playlistId, [FromForm] List<int> booksIds)
        {
            var result = await _communityService.AddBooksToPlaylist(playlistId, booksIds, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("removeBooksFromPlaylist")]
        public async Task<IActionResult> RemoveBooksFromPlaylistAsync([FromForm] int playlistId, [FromForm] List<int> booksIds)
        {
            var result = await _communityService.RemoveBooksFromPlaylist(playlistId, booksIds, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("addBooksToDiscussion")]
        public async Task<IActionResult> AddBooksToDiscussionAsync([FromForm] int discussionId, [FromForm] List<int> booksIds)
        {
            var result = await _communityService.AddBooksToDiscussion(discussionId, booksIds, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("removeBooksFromDiscussion")]
        public async Task<IActionResult> RemoveBooksFromDiscussionAsync([FromForm] int discussionId, [FromForm] List<int> booksIds)
        {
            var result = await _communityService.RemoveBooksFromDiscussion(discussionId, booksIds, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpGet("allDiscussions")]
        public async Task<IActionResult> GetAllDiscussionsAsync([FromQuery] int page, DiscussionArticleNewsTag tag)
        {
            var result = await _communityService.GetAllDiscussionsAsync(page, tag);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("userReviews")]
        public async Task<IActionResult> GeUserReviewsAsync([FromForm] string userId, [FromForm] int page)
        {
            var result = await _communityService.GetUserReviews(userId, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("userPlaylists")]
        public async Task<IActionResult> GeUserPlaylistsAsync([FromForm] string userId, [FromForm] int page)
        {
            var result = await _communityService.GetUserPlaylists(userId, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpGet("userDiscussions")]
        public async Task<IActionResult> GeUserDiscussionsAsync([FromForm] string userId, [FromForm] int page)
        {
            var result = await _communityService.GetUserDiscussions(userId, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int postId)
        {
            var result = await _communityService.Delete(postId);

            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Result);
        }
        [HttpPost("comment")]
        public async Task<IActionResult> AddCommentAsync(CommentDto dto)
        {
            var result = await _communityService.AddCommentAsync(dto);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpGet("comment")]
        public async Task<IActionResult> GetCommentAsync([FromQuery] int commentId)
        {
            var result = await _communityService.GetCommentAsync(commentId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpDelete("comment")]
        public async Task<IActionResult> DeleteCommentAsync([FromQuery] int commentId)
        {
            var result = await _communityService.DeleteCommentAsync(commentId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpGet("postComments")]
        public async Task<IActionResult> GetPostCommentsAsync([FromQuery] int postId, [FromQuery] int page)
        {
            var result = await _communityService.GetPostCommentsAsync(postId, page);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }

        [HttpPut("likeComment")]
        public async Task<IActionResult> LikeCommentsAsync([FromQuery] int commentId)
        {
            var result = await _communityService.LikeCommentsAsync(commentId);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }
        [HttpPut("comment")]
        public async Task<IActionResult> UpdateCommentsAsync([FromForm] int commentId, [FromForm] string content)
        {
            var result = await _communityService.UpdateCommentAsync(commentId, content);
            if (result.Succeeded == false)
                return BadRequest(result.ErrorMessage);
            return Created((string?)null, result.Result);
        }

    }
}
