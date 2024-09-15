using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.User;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IAuthorService
    {
        Task<GenericResultDto<PagedResult<AuthorNameAndIdDto>>> GetAllNamesAndIds(int page);
        Task<GenericResultDto<AuthorPagesCount>> GetAuthorsPagesCount();
        Task<GenericResultDto<PagedResult<AuthorDto>>> GetAll(int page);
        Task<GenericResultDto<PagedResult<BookDto>>> GetAuthorBooks(int authorId, int page);
        Task<GenericResultDto<AuthorDto>> GetByIdAsync(int authorId);
        Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName);
        Task<GenericResultDto<AuthorDto?>> AddAsync(IFormFile pic, AuthorDto newAuthor);
        Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId);
        Task<GenericResultDto<AuthorDto>> UpdateAllAsync(IFormFile? pic, AuthorWithoutPicDto dto);
        Task<GenericResultDto<string>> Delete(int authorId);
        void Attach(IEnumerable<Author> authors);
        Task<GenericResultDto<IQueryable<AuthorDto>>> GetRangeAsync(HashSet<int> authorsIds);
        int GetCount();

    }
}
