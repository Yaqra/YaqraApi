using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;

namespace YaqraApi.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationContext _context;

        public RecommendationRepository(ApplicationContext context)
        {
            _context = context;
        }
        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task AddAsync(string userId, int genreId)
        {
            var obj = new RecommendationStatistics
            {
                UserId = userId,
                GenreId = genreId
            };
            await _context.RecommendationStatistics.AddAsync(obj);

            await SaveChangesAsync();
        }

        public async Task<RecommendationStatistics?> GetAsync(string userId, int genreId)
        {
            var obj = await _context.RecommendationStatistics
                .SingleOrDefaultAsync(r => r.UserId == userId && r.GenreId == genreId);
            return obj;
        }

        public async Task Update(RecommendationStatistics obj)
        {
            _context.RecommendationStatistics.Update(obj);
            await SaveChangesAsync();
        }

        public async Task<RecommendationStatistics> IncrementPoints(RecommendationStatistics obj)
        {
            obj.Points++;
            await Update(obj);
            return obj;
        }

        public async Task<RecommendationStatistics> DecrementPoints(RecommendationStatistics obj)
        {
            obj.Points--;
            await Update(obj);
            return obj;
        }

        public void Delete(RecommendationStatistics obj)
        {
            _context.RecommendationStatistics.Remove(obj);
            SaveChanges();
        }

        public async Task<List<RecommendationStatistics>> GetByUserId(string userId)
        {
            var objects = await _context.RecommendationStatistics
                .Where(r=>r.UserId == userId)
                .ToListAsync();
            return objects;
        }
    }
}
