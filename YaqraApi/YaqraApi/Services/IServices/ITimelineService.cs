using System.Collections;
using YaqraApi.DTOs;

namespace YaqraApi.Services.IServices
{
    public interface ITimelineService
    {
        Task<GenericResultDto<ArrayList>> GetTimeline(int page, bool followings, string userId);
    }
}
