using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.User;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IAuthorService
    {
        public Task<GenericResultDto<List<AuthorNameAndIdDto>>> GetAllNamesAndIds();
        public Task<GenericResultDto<List<AuthorDto>>> GetAll();
        public Task<GenericResultDto<AuthorDto>> GetByIdAsync(int authorId);
        public Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName);
        public Task<GenericResultDto<AuthorDto?>> AddAsync(IFormFile pic, AuthorDto newAuthor);
        Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId);
        Task<GenericResultDto<AuthorDto>> UpdateAllAsync(IFormFile? pic, AuthorWithoutPicDto dto);
        Task<GenericResultDto<string>> Delete(int authorId);
    }
}
