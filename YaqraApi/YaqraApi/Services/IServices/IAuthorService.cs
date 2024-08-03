using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.User;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IAuthorService
    {
        Task<GenericResultDto<List<AuthorNameAndIdDto>>> GetAllNamesAndIds(int page);
        Task<GenericResultDto<AuthorPagesCount>> GetAuthorsPagesCount();
        Task<GenericResultDto<List<AuthorDto>>> GetAll(int page);
        Task<GenericResultDto<List<BookDto>>> GetAuthorBooks(int authorId, int page);
        Task<GenericResultDto<AuthorDto>> GetByIdAsync(int authorId);
        Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName, int page);
        Task<GenericResultDto<AuthorDto?>> AddAsync(IFormFile pic, AuthorDto newAuthor);
        Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId);
        Task<GenericResultDto<AuthorDto>> UpdateAllAsync(IFormFile? pic, AuthorWithoutPicDto dto);
        Task<GenericResultDto<string>> Delete(int authorId);
        void Attach(IEnumerable<Author> authors);
    }
}
