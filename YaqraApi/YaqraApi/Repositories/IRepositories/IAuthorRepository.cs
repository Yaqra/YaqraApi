using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IAuthorRepository
    {
        Task<IQueryable<AuthorNameAndIdDto>> GetAllNamesAndIds(int page);
        Task<List<Book>> GetAuthorBooks(int authorId, int page);
        Task<IQueryable<Author>> GetAll(int page);
        Task<Author> GetByIdAsync(int authorId);
        Task<IQueryable<Author>> GetByName(string authorName, int page);
        Task<Author?> AddAsync(Author newAuthor);
        void UpdateAll(Author editedAuthor);//all author details
        void Delete(Author author);
        int GetCount();
        void Attach(IEnumerable<Author> authors);
        Task<List<int>?> GetAuthorBooksIds(int authorId);
        Task<List<decimal>?> GetAuthorBooksRates(List<int> booksIds);
        Task<IQueryable<Author>> GetRangeAsync(HashSet<int> authorsIds);
    }
}
