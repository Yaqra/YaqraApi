using Microsoft.EntityFrameworkCore;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.Helpers;
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

        public async Task<IQueryable<Author>> GetAll(int page)
        {
            return _context.Authors
                .Skip((page - 1) * Pagination.Authors).Take(Pagination.Authors)
                .AsNoTracking();

        }

        public async Task<IQueryable<AuthorNameAndIdDto>> GetAllNamesAndIds(int page)
        {
            var authors = _context.Authors
                .Select(a => new AuthorNameAndIdDto{AuthorId = a.Id,AuthorName = a.Name })
                .Skip((page - 1)*Pagination.AuthorNamesAndIds).Take(Pagination.AuthorNamesAndIds)
                .AsNoTracking();
            return authors;
        }

        public async Task<Author?> GetByIdAsync(int authorId)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == authorId);
            return author;
        }

        public async Task<IQueryable<Author>> GetByName(string authorName, int page)
        {
            var authors = _context.Authors
                .Where(a => a.Name.Contains(authorName))
                .Skip((page - 1) * Pagination.Authors).Take(Pagination.Authors);
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

        public int GetCount()
        {
            return _context.Authors.Count();
        }

        public async Task<List<Book>> GetAuthorBooks(int authorId, int page)
        {
            var author = _context.Authors
                .Include(a=>a.Books)
                    .ThenInclude(b=>b.Genres)
                .Include(b=>b.Books)
                    .ThenInclude(b=>b.Authors)
                .SingleOrDefault(a => a.Id == authorId);
            
            var books = author.Books;

            return books == null ? new List<Book>() : books.ToList();
        }

        public void Attach(IEnumerable<Author> authors)
        {
            foreach (var author in authors)
                _context.Authors.Attach(author);
        }

        public async Task<List<int>?> GetAuthorBooksIds(int authorId)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .SingleOrDefaultAsync(a => a.Id == authorId);
            if (author == null)
                return null;
            return author.Books.Select(b => b.Id).ToList();
        }

        public async Task<List<decimal>?> GetAuthorBooksRates(List<int> booksIds)
        {
            return _context.Reviews.Where(r => booksIds.Contains(r.BookId)).Select(r => r.Rate).ToList();
        }
    }
}
