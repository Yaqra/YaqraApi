using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IRecommendationService
    {
        Task<RecommendationStatistics?> GetAsync(string userId, int genreId);
        Task IncrementPoints(string userId, int genreId);
        Task DecrementPoints(string userId, int genreId);
        Task<GenericResultDto<List<BookDto>?>> RecommendBooks(string userId);
    }
}
