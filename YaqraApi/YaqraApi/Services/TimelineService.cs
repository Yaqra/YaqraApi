using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using YaqraApi.DTOs;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly ApplicationContext _context;
        private readonly IRecommendationService _recommendationService;
        private readonly ICommunityService _communityService;

        public TimelineService(ApplicationContext context, IRecommendationService recommendationService, ICommunityService communityService)
        {
            _context = context;
            _recommendationService = recommendationService;
            _communityService = communityService;
        }
        public async Task<GenericResultDto<ArrayList>> GetTimeline(int page, bool followings, string userId)
        {
            var user = await _context.Users
                .Include(u=>u.Followings)
                .SingleOrDefaultAsync(u=>u.Id==userId);
            if (user == null)
                return new GenericResultDto<ArrayList> { Succeeded = false, ErrorMessage = "user not found" };
            var arr = new ArrayList();
            if(followings == true)
            {
                //get followings timeline
                var followingsIds = user.Followings.Select(f => f.Id);
                arr = (await _communityService.GetFollowingsPostsAsync(followingsIds, page)).Result;
            }
            else 
                //get default timeline
                arr = (await _communityService.GetPostsAsync(page)).Result;

            //add recommended books
            var recommendedBooks = (await _recommendationService.RecommendBooks(userId)).Result;
            if(recommendedBooks.IsNullOrEmpty() == false)
                arr.AddRange(recommendedBooks);
            return new GenericResultDto<ArrayList> { Succeeded = true, Result = arr };
        }
    }
}
