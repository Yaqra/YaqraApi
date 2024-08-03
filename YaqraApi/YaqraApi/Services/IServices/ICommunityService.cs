using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface ICommunityService
    {
        Task<GenericResultDto<ReviewDto>> AddReviewAsync(ReviewDto review);
        Task<GenericResultDto<ReviewDto>> GetReviewAsync(int reviewId);
        Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(ReviewDto editedReview);
        Task<GenericResultDto<string>> Delete(int postId);
    }
}
