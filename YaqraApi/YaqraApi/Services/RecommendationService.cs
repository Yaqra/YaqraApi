using Microsoft.IdentityModel.Tokens;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IGenreService _genreService;

        public RecommendationService(IRecommendationRepository recommendationRepository, IGenreService genreService)
        {
            _recommendationRepository = recommendationRepository;
            _genreService = genreService;
        }

        public async Task DecrementPoints(string userId, int genreId)
        {
            var obj = await _recommendationRepository.GetAsync(userId, genreId);
            if (obj == null)
                return;
            if (obj.Points-1 <= 0)
                _recommendationRepository.Delete(obj);
            else
                obj = await _recommendationRepository.DecrementPoints(obj);
        }

        public async Task<RecommendationStatistics?> GetAsync(string userId, int genreId)
        {
            return await _recommendationRepository.GetAsync(userId, genreId);
        }

        public async Task IncrementPoints(string userId, int genreId)
        {
            var obj = await _recommendationRepository.GetAsync(userId, genreId);
            if (obj == null)
                await _recommendationRepository.AddAsync(userId, genreId);
            else
                await _recommendationRepository.IncrementPoints(obj);
        }

        public async Task<GenericResultDto<List<BookDto>?>> RecommendBooks(string userId)
        {
            var recommendationStatistics = (await _recommendationRepository.GetByUserId(userId))
                                            .OrderByDescending(r=>r.Points)
                                            .Take(3);
            var books = new List<BookDto>();
            if (recommendationStatistics.IsNullOrEmpty())
                return new GenericResultDto<List<BookDto>?> { Succeeded = true, Result = books};

            var totalPoints = recommendationStatistics.Sum(r => r.Points);
            foreach (var obj in recommendationStatistics)
            {
                obj.Points = (int) Math.Round((double)(obj.Points) / (double)totalPoints * (double)Pagination.RecommendedBooks);
                var randomizationBooksResult = await _genreService.RandomizeBooksBasedOnGenre(obj.GenreId, obj.Points);
                if(randomizationBooksResult.Succeeded == true)
                {
                    if(randomizationBooksResult.Result != null)
                        books.AddRange(randomizationBooksResult.Result);
                }
            }
            // check repetition
            var set = new HashSet<int>(books.Select(b => b.Id));
            var result = new List<BookDto?>();
            foreach (var id in set)
            {
                result.Add(books.FirstOrDefault(b => b.Id == id));
            }
            return new GenericResultDto<List<BookDto>?> { Succeeded = true, Result = result};
        }
    }
}
