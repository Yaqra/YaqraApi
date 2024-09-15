using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

namespace YaqraApi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationContext _context;
        private readonly Mapper _mapper;

        public BookRepository(ApplicationContext context)
        {
            _context = context;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public void Attach(IEnumerable<Book> books)
        {
            foreach (var book in books)
                _context.Books.Attach(book);
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

        public async Task<IQueryable<Book>> GetByTitle(string bookName)
        {
            var books = _context.Books
                .Where(a => a.Title.Contains(bookName))
                .Take(Pagination.Books);
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

        public int GetRecentCount()
        {
            return _context.Books
                .Where(b => b.AddedDate >= DateTime.UtcNow.AddDays(-7))
                .Count();
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
        public int GetReviewsCount(int bookId)
        {
            return _context.Reviews
           .Where(r => r.BookId == bookId)
           .Count();
        }
        public async Task<List<Review>> GetReviews(int bookId, int page, SortType type, ReviewsSortField field)
        {

            switch (field)
            {
                case ReviewsSortField.LikeCount:
                    return type == SortType.Ascending ?
                        _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderBy(r => r.LikeCount)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList() : _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderByDescending(r => r.LikeCount)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList();
                case ReviewsSortField.CreatedDate:
                    return type == SortType.Ascending ?
                        _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderBy(r => r.CreatedDate)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList() : _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderByDescending(r => r.CreatedDate)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList();
                case ReviewsSortField.Rate:
                    return type == SortType.Ascending ?
                        _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderBy(r => r.Rate)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList() : _context.Reviews
                        .AsNoTracking()
                        .Where(r => r.BookId == bookId)
                        .Include(r => r.User)
                        .OrderByDescending(r => r.Rate)
                        .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                        .ToList();
                default:
                    return _context.Reviews
                   .AsNoTracking()
                   .Where(r => r.BookId == bookId)
                   .Include(r => r.User)
                   .OrderBy(r => r.Rate)
                   .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                   .ToList();
            }


        }
        public async Task<int> FindBooksCount(BookFinderDto dto, IBookProxyService bookProxyService)
        {
            if (dto.MinimumRate == null && dto.AuthorIds == null && dto.GenreIds == null)
                return GetCount();

            IQueryable<Book> books = _context.Books
                    .Include(b => b.Genres)
                    .Include(b => b.Authors);

            if (dto.GenreIds.IsNullOrEmpty() == false)
                books = books.Where(b => b.Genres.Any(g => dto.GenreIds.Contains(g.Id)));

            if (dto.AuthorIds.IsNullOrEmpty() == false)
                books = books.Where(b => b.Authors.Any(a => dto.AuthorIds.Contains(a.Id)));

            var result = _mapper.Map<List<BookDto>>(books.ToList());

            if (dto.MinimumRate != null)
            {
                var elementsToRemove = new List<BookDto>();
                foreach (var b in result)
                {
                    var bookRate = await bookProxyService.CalculateRate(b.Id);
                    if (bookRate == null)
                    {
                        elementsToRemove.Add(b);
                        continue;
                    }

                    if (bookRate < dto.MinimumRate)
                    {
                        elementsToRemove.Add(b);
                        continue;
                    }
                }
                foreach (var item in elementsToRemove)
                {
                    result.Remove(item);
                }
            }

            return result.Count();
        }
        public async Task<List<BookDto>> FindBooks(BookFinderDto dto, IBookProxyService bookProxyService)
        {
            if (dto.MinimumRate == null && dto.AuthorIds == null && dto.GenreIds == null)
            {
                var booksDto = _mapper.Map<List<BookDto>>(await GetAll(dto.Page));
                foreach (var book in booksDto)
                {
                    book.Rate = BookHelpers.FormatRate(await bookProxyService.CalculateRate(book.Id));
                }
                return booksDto;
            }

            IQueryable<Book> books = _context.Books
                    .Include(b => b.Genres)
                    .Include(b => b.Authors);

            if (dto.GenreIds.IsNullOrEmpty() == false)
                books = books.Where(b => b.Genres.Any(g => dto.GenreIds.Contains(g.Id)));

            if (dto.AuthorIds.IsNullOrEmpty() == false)
                   books = books.Where(b => b.Authors.Any(a => dto.AuthorIds.Contains(a.Id)));

            var result = _mapper.Map<List<BookDto>>(books.ToList());

            if (dto.MinimumRate != null)
            {
                var elementsToRemove = new List<BookDto>();
                foreach (var b in result)
                {
                    var bookRate = await bookProxyService.CalculateRate(b.Id);
                    if (bookRate == null)
                    {
                        elementsToRemove.Add(b);
                        continue;
                    }

                    if (bookRate < dto.MinimumRate)
                    {
                        elementsToRemove.Add(b);
                        continue;
                    }

                    b.Rate = BookHelpers.FormatRate(bookRate);
                }
                foreach (var item in elementsToRemove)
                {
                    result.Remove(item);
                }
            }         

            return result
                .Skip((dto.Page - 1) * Pagination.Books).Take(Pagination.Books)
                .ToList();
        }

        public async Task<List<Book>> GetTrendingBooks()
        {
            var books = await _context.TrendingBooks
                .Include(b => b.Book)
                .Where(b => b.AddedDate >= DateTime.UtcNow.AddDays(-7))
                .GroupBy(b => b.BookId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.FirstOrDefault().Book) // Select the first book in each group
                .ToListAsync();

            return books
                .Take(Pagination.TrendingBooks).ToList();
        }

        public async Task AddTrendingBook(TrendingBook trending)
        {
            await _context.TrendingBooks.AddAsync(trending);
            await SaveChangesAsync();
        }

        public int GetUpcomingBooksCount()
        {
            return _context.Books
                .Where(b => b.AddedDate > DateTime.UtcNow)
                .Count();
        }

        public async Task<List<Book>> GetUpcomingBooks(int page)
        {
            return await _context.Books.Where(b => b.AddedDate > DateTime.UtcNow)
                                    .Skip((page - 1) * Pagination.Books).Take(Pagination.Books)
                                    .ToListAsync();
        }

        public async Task<IQueryable<Book>> GetRange(HashSet<int> booksIds)
        {
            return _context.Books
                .Where(b => booksIds.Contains(b.Id));
        }

        public async Task LoadGenres(Book book)
        {
            await _context.Entry(book).Collection(b => b.Genres).LoadAsync();
        }

        public async Task<int> GetBookReviewsCount(int bookId)
        {
            return _context.Reviews.Where(r=>r.BookId==bookId).Count();
        }
    }
}
