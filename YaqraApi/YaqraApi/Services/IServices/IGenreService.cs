using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Genre;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IGenreService
    {
        Task<GenericResultDto<List<GenreDto>>> GetAllAsync(int page);
        Task<GenericResultDto<int>> GetPagesCount();
        Task<GenericResultDto<GenreDto>> GetByIdAsync(int id);
        Task<GenericResultDto<GenreDto>> GetByNameAsync(string name);
        Task<GenericResultDto<GenreDto>> AddAsync(string genreName);
        Task<GenericResultDto<GenreDto>> UpdateAsync(int currentGenreId, string newGenreName);
        Task<GenericResultDto<string>> DeleteAsync(int id);
        Task<GenericResultDto<List<BookDto>?>> RandomizeBooksBasedOnGenre(int genreId, int count);
        Task<GenericResultDto<IQueryable<GenreDto>>> GetRangeAsync(HashSet<int> genreIds);
        void Attach(IEnumerable<Genre> genres);
    }
}
