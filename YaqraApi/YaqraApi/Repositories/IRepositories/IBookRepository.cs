using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IBookRepository
    {
        Task<IQueryable<BookTitleAndIdDto>> GetAllTitlesAndIds(int page);
        Task<List<Book>> GetAll(int page);
        int GetCount();
        Task<Book> GetByIdAsync(int bookId);
        Task<IQueryable<Book>> GetByTitle(string bookName, int page);
        Task<Book?> AddAsync(Book newBook);
        void UpdateAll(Book editedBook);//all Book details
        void Delete(Book Book);
        Task<IQueryable<Book>> GetRecent(int page);
        Task<List<decimal>> GetBookRates(int bookId);

    }
}
