using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IAuthorRepository
    {
        public Task<IQueryable<AuthorNameAndIdDto>> GetAllNamesAndIds(int page);
        public Task<IQueryable<Author>> GetAll(int page);
        public Task<Author> GetByIdAsync(int authorId);
        public Task<IQueryable<Author>> GetByName(string authorName, int page);
        public Task<Author?> AddAsync(Author newAuthor);
        public void UpdateAll(Author editedAuthor);//all author details
        public void Delete(Author author);
    }
}
