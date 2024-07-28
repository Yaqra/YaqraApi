using YaqraApi.DTOs.Book;
using YaqraApi.DTOs;

namespace YaqraApi.Services.IServices
{
    public interface IBookService
    {
        public Task<GenericResultDto<List<BookTitleAndIdDto>>> GetAllTitlesAndIds(int page);
        public Task<GenericResultDto<List<BookDto>>> GetAll(int page);
        public Task<GenericResultDto<BookPagesCount>> GetBooksPagesCount();
        public Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId);
        public Task<GenericResultDto<List<BookDto>>> GetByTitle(string bookName, int page);
        public Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto);
        Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId);
        Task<GenericResultDto<BookDto>> UpdateAllAsync(IFormFile? img, BookWithoutImageDto dto);
        Task<GenericResultDto<string>> Delete(int bookId);
    }
}
