using YaqraApi.DTOs.Community;
using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface ICommunityRepository
    {
        Task<Review?> AddReviewAsync(Review review);
        Task<Review?> GetReviewAsync(int reviewId);
        Task<Review?> UpdateReviewAsync(Review editedReview);
        void Delete(Post post);
    }
}
