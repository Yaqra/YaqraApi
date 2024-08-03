using YaqraApi.DTOs.Book;
using YaqraApi.DTOs;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IBookService
    {
        Task<GenericResultDto<List<BookTitleAndIdDto>>> GetAllTitlesAndIds(int page);
        Task<GenericResultDto<List<BookDto>>> GetAll(int page);
        Task<GenericResultDto<BookPagesCount>> GetBooksPagesCount();
        Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId);
        Task<GenericResultDto<List<BookDto>>> GetByTitle(string bookName, int page);
        Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto);
        Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId);
        Task<GenericResultDto<BookDto>> UpdateAllAsync(IFormFile? img, BookWithoutImageDto dto);
        Task<GenericResultDto<string>> Delete(int bookId);
        Task<GenericResultDto<List<BookDto>>> GetRecent(int page);
        Task<GenericResultDto<BookDto>> AddGenresToBook(List<int> genresIds, int bookId);
        Task<GenericResultDto<BookDto>> RemoveGenresFromBook(List<int> genresIds, int bookId);
        Task<GenericResultDto<BookDto>> AddAuthorsToBook(List<int> AuthorsIds, int bookId);
        Task<GenericResultDto<BookDto>> RemoveAuthorsFromBook(List<int> authorIds, int bookId);
    }
}
