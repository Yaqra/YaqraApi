using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IRecommendationRepository
    {
        Task AddAsync(string userId, int genreId);
        Task<RecommendationStatistics?> GetAsync(string userId, int genreId);
        Task Update(RecommendationStatistics obj);
        Task<RecommendationStatistics> IncrementPoints(RecommendationStatistics obj);
        Task<RecommendationStatistics> DecrementPoints(RecommendationStatistics obj);
        Task<List<RecommendationStatistics>> GetByUserId(string userId);
        void Delete(RecommendationStatistics obj);
    }
}
