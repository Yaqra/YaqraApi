using Microsoft.EntityFrameworkCore;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;

namespace YaqraApi.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationContext _context;

        public AuthorRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Author?> AddAsync(Author newAuthor)
        {
            await _context.Authors.AddAsync(newAuthor);
            await SaveChangesAsync();

            _context.Entry(newAuthor).State = EntityState.Detached;

            return newAuthor.Id==0? null : newAuthor;
        }

        public async Task<IQueryable<Author>> GetAll()
        {
            return _context.Authors.AsNoTracking();

        }

        public async Task<IQueryable<AuthorNameAndIdDto>> GetAllNamesAndIds()
        {
            var authors = _context.Authors.Select(a => new AuthorNameAndIdDto{AuthorId = a.Id,AuthorName = a.Name }).AsNoTracking();
            return authors;
        }

        public async Task<Author?> GetByIdAsync(int authorId)
        {
            var author = await _context.Authors.AsNoTracking().SingleOrDefaultAsync(a => a.Id == authorId);
            return author;
        }

        public async Task<IQueryable<Author>> GetByName(string authorName)
        {
            var authors = _context.Authors.Where(a => a.Name.Contains(authorName));
            return authors;
        }
        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdateAll(Author editedAuthor)
        {
            _context.Authors.Update(editedAuthor);
            SaveChanges();
            _context.Entry(editedAuthor).State = EntityState.Detached;
        }

        public void Delete(Author author)
        {
            if (File.Exists(author.Picture))
                File.Delete(author.Picture);
            _context.Authors.Remove(author);
            SaveChanges();
        }
    }
}
