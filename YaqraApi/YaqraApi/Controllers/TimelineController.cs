using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using YaqraApi.DTOs.Timeline;
using YaqraApi.Helpers;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;

        public TimelineController(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }
        [HttpGet]
        public async Task<IActionResult> Get(bool followings, int page)
        {
            var dto = new GetTimelineDto { Followings = followings, page = page };
            var result = await _timelineService.GetTimeline(dto.page, dto.Followings, UserHelpers.GetUserId(User));
            if (result.Succeeded == false)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
