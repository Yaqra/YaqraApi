using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Linq;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
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
            var arr = new ArrayList();
            if(userId == null || followings == false)
                arr = (await _communityService.GetPostsAsync(page)).Result;
            else
            {
                //get followings timeline
                var user = await _context.Users
                    .Include(u => u.Followings)
                    .SingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return new GenericResultDto<ArrayList> { Succeeded = false, ErrorMessage = "user not found" };

                var followingsIds = user.Followings.Select(f => f.Id);
                arr = (await _communityService.GetFollowingsPostsAsync(followingsIds, page)).Result;
            }

            if(userId != null)
            {
                var postsIds = arr.OfType<PostDto>().DistinctBy(p => p.Id).Select(p => p.Id).ToList();
                var likedPostsIds = await _communityService.ArePostsLiked(postsIds, userId);
                for (int i = 0; i < arr.Count; i++)
                {
                    if (arr[i] is PostDto post)
                    {
                        if (likedPostsIds.Contains(post.Id) == true)
                        {
                            post.IsLiked = true;
                        }
                    }
                }

                //add recommended books
                var recommendedBooks = (await _recommendationService.RecommendBooks(userId)).Result;
                if (recommendedBooks.IsNullOrEmpty() == false)
                    arr.AddRange(recommendedBooks);
            }

            return new GenericResultDto<ArrayList> { Succeeded = true, Result = arr };
        }
    }
}
