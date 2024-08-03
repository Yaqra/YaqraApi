using Microsoft.EntityFrameworkCore;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;

namespace YaqraApi.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationContext _context;
        public GenreRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<List<Genre>> GetAllAsync(int page)
        {
            var genres = await _context.Genres
                .Skip((page - 1)*Pagination.Genres).Take(Pagination.Genres)
                .AsNoTracking()
                .ToListAsync();
            return genres;
        }
        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task<Genre> GetByIdAsync(int id)
        {
            var genre = await _context.Genres.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
            return genre;
        }
        public async Task<Genre> GetByNameAsync(string name)
        {
            var genre = await _context.Genres.AsNoTracking().SingleOrDefaultAsync(g => g.Name == name);
            return genre;
        }
        public async Task<Genre> AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await SaveChangesAsync();
            return genre.Id == 0? null : genre;
        }
        public void Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            SaveChanges();
        }
        public async Task<Genre> UpdateAsync(int currentGenreId, Genre editedGenre)
        {
            var genre = await GetByIdAsync(currentGenreId);
            if (genre == null)
                return null;
            editedGenre.Id= genre.Id;
            _context.Genres.Update(editedGenre);
            SaveChanges();
            return editedGenre;
        }

        public int GetCount()
        {
            return _context.Genres.Count();
        }

        public void Attach(IEnumerable<Genre> genres)
        {
            foreach (var genre in genres)
                _context.Genres.Attach(genre);
        }
    }
}
