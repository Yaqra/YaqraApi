using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;

namespace YaqraApi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationContext _context;

        public BookRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Book?> AddAsync(Book newBook)
        {
            await _context.Books.AddAsync(newBook);
            await SaveChangesAsync();

            _context.Entry(newBook).State = EntityState.Detached;

            return newBook.Id == 0 ? null : newBook;
        }

        public void Delete(Book Book)
        {
            if (File.Exists(Book.Image))
                File.Delete(Book.Image);
            _context.Books.Remove(Book);
            SaveChanges();
        }

        public async Task<List<Book>> GetAll(int page)
        {
            return _context.Books
                .Include(b=>b.Authors)
                .Include(b=>b.Genres)
                .Skip((page-1)*Pagination.BookTitlesAndIds).Take(Pagination.BookTitlesAndIds)
                .AsNoTracking().ToList();
        }

        public async Task<IQueryable<BookTitleAndIdDto>> GetAllTitlesAndIds(int page)
        {
            var books = _context.Books
                .Select(a => new BookTitleAndIdDto { Id = a.Id, Title = a.Title })
                .Skip((page-1)*Pagination.Books).Take(Pagination.Books)
                .AsNoTracking();
            return books;
        }

        public async Task<Book> GetByIdAsync(int bookId)
        {
            var books = await _context.Books
                .AsNoTracking()
                .Include(b=>b.Authors)
                .Include(b=>b.Genres)
                .SingleOrDefaultAsync(a => a.Id == bookId);
            return books;
        }

        public async Task<IQueryable<Book>> GetByTitle(string bookName, int page)
        {
            var books = _context.Books
                .Where(a => a.Title.Contains(bookName))
                .Skip((page-1)*Pagination.Books).Take(Pagination.Books);
            return books;
        }

        public void UpdateAll(Book editedBook)
        {
            _context.Books.Update(editedBook);
            SaveChanges();
            _context.Entry(editedBook).State = EntityState.Detached;
        }
        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }

        public int GetCount()
        {
            return _context.Books.Count();
        }

        public async Task<IQueryable<Book>> GetRecent(int page)
        {
            var books = _context.Books
                .Where(b=>b.AddedDate>= DateTime.UtcNow.AddDays(-7))
                .Include(b=>b.Genres)
                .Include(b=>b.Authors)
                .Skip((page-1)*Pagination.Books).Take(Pagination.Books);
            return books;
        }

        public async Task<List<decimal>> GetBookRates(int bookId)
        {
            return _context.Reviews.Where(r => r.BookId == bookId).Select(r => r.Rate).ToList();
        }
    }
}
