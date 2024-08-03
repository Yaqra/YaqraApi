using Microsoft.EntityFrameworkCore;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;

namespace YaqraApi.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly ApplicationContext _context;

        public CommunityRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<Review?> AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await SaveChangesAsync();

            _context.Entry(review).State = EntityState.Detached;

            return review.Id == 0 ? null : review;
        }

        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<Review?> GetReviewAsync(int reviewId)
        {
            return await (_context.Reviews.SingleOrDefaultAsync(r => r.Id == reviewId));
        }

        public void Delete(Post post)
        {
            _context.Posts.Remove(post);
            SaveChanges();
        }

        public async Task<Review?> UpdateReviewAsync(Review editedReview)
        {
            _context.Reviews.Update(editedReview);
            SaveChanges();
            return editedReview;
        }
    }
}
