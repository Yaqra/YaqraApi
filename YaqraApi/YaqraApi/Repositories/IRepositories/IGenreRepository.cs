using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync();
        Task<Genre> GetByIdAsync(int id);
        Task<Genre> GetByNameAsync(string name);
        Task<Genre> AddAsync(Genre genre);
        Task<bool> DeleteAsync(int id);
        Task<Genre> UpdateAsync(int currentGenreId, Genre editedGenre);
    }
}
