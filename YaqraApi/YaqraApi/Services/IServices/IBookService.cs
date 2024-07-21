using YaqraApi.DTOs.Book;
using YaqraApi.DTOs;

namespace YaqraApi.Services.IServices
{
    public interface IBookService
    {
        public Task<GenericResultDto<List<BookTitleAndIdDto>>> GetAllTitlesAndIds();
        public Task<GenericResultDto<List<BookDto>>> GetAll();
        public Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId);
        public Task<GenericResultDto<List<BookDto>>> GetByTitle(string bookName);
        public Task<GenericResultDto<BookDto?>> AddAsync(IFormFile img, BookDto newBook);
        Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId);
        Task<GenericResultDto<BookDto>> UpdateAllAsync(IFormFile? img, BookWithoutImageDto dto);
        Task<GenericResultDto<string>> Delete(int bookId);
    }
}
