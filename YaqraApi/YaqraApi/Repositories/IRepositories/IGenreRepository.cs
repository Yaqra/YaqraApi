using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync(int page);
        int GetCount();
        Task<Genre> GetByIdAsync(int id);
        Task<Genre> GetByNameAsync(string name);
        Task<Genre> AddAsync(Genre genre);
        void Delete(Genre genre);
        Task<Genre> UpdateAsync(int currentGenreId, Genre editedGenre);
    }
}
