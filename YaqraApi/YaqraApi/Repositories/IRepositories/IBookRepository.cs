using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Services.IServices;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IBookRepository
    {
        Task<IQueryable<BookTitleAndIdDto>> GetAllTitlesAndIds(int page);
        Task<List<Book>> GetAll(int page);
        int GetCount();
        Task<Book> GetByIdAsync(int bookId);
        Task<IQueryable<Book>> GetByTitle(string bookName);
        Task<Book?> AddAsync(Book newBook);
        void UpdateAll(Book editedBook);//all Book details
        void Delete(Book Book);
        int GetRecentCount();
        Task<IQueryable<Book>> GetRecent(int page);
        Task<List<decimal>> GetBookRates(int bookId);
        void Attach(IEnumerable<Book> books);
        int GetReviewsCount(int bookId);
        Task<List<Review>> GetReviews(int bookId, int page, SortType type, ReviewsSortField field);
        Task<int> FindBooksCount(BookFinderDto dto, IBookProxyService bookProxyService);
        Task<List<BookDto>> FindBooks(BookFinderDto dto, IBookProxyService bookProxyService);
        Task<List<Book>> GetTrendingBooks();
        Task AddTrendingBook(TrendingBook trending);
        int GetUpcomingBooksCount();
        Task<List<Book>> GetUpcomingBooks(int page);
        Task<IQueryable<Book>> GetRange(HashSet<int> booksIds);
        Task LoadGenres(Book book);
        Task<int> GetBookReviewsCount(int bookId);
    }
}
