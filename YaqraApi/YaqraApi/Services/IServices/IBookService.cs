using YaqraApi.DTOs.Book;
using YaqraApi.DTOs;
using YaqraApi.Models;
using YaqraApi.DTOs.Community;
using Microsoft.AspNetCore.Mvc;
using YaqraApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace YaqraApi.Services.IServices
{
    public interface IBookService
    {
        Task<GenericResultDto<PagedResult<BookTitleAndIdDto>>> GetAllTitlesAndIds(int page);
        Task<GenericResultDto<PagedResult<BookDto>>> GetAll(int page);
        Task<GenericResultDto<BookPagesCount>> GetBooksPagesCount();
        Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId);
        Task<GenericResultDto<List<BookDto>>> GetByTitle(string bookName);
        Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto);
        Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId);
        Task<GenericResultDto<BookDto>> UpdateAllAsync(IFormFile? img, BookWithoutImageDto dto);
        Task<GenericResultDto<string>> Delete(int bookId);
        Task<GenericResultDto<PagedResult<BookDto>>> GetRecent(int page);
        Task<GenericResultDto<BookDto>> AddGenresToBook(HashSet<int> genresIds, int bookId);
        Task<GenericResultDto<BookDto>> RemoveGenresFromBook(HashSet<int> genresIds, int bookId);
        Task<GenericResultDto<BookDto>> AddAuthorsToBook(HashSet<int> AuthorsIds, int bookId);
        Task<GenericResultDto<BookDto>> RemoveAuthorsFromBook(HashSet<int> authorIds, int bookId);
        void Attach(IEnumerable<Book> books);
        Task<GenericResultDto<PagedResult<ReviewDto>>> GetReviews(int bookId, int page, SortType type, ReviewsSortField field);
        Task<GenericResultDto<PagedResult<BookDto>>> FindBooks(BookFinderDto dto);
        Task<GenericResultDto<List<BookDto>>> GetTrendingBooks();
        Task AddTrendingBook(int bookId);
        Task<GenericResultDto<PagedResult<BookDto>>> GetUpcomingBooks(int page);
        Task<GenericResultDto<IQueryable<Book>>> GetRangeAsync(HashSet<int> booksIds);
        Task LoadGenres(Book book);
        bool IsEntityTracked(Book book);

    }
}
