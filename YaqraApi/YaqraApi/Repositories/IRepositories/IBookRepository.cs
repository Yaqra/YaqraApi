using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;

namespace YaqraApi.Repositories.IRepositories
{
    public interface IBookRepository
    {
        public Task<IQueryable<BookTitleAndIdDto>> GetAllTitlesAndIds(int page);
        public Task<IQueryable<Book>> GetAll(int page);
        public int GetCount();
        public Task<Book> GetByIdAsync(int bookId);
        public Task<IQueryable<Book>> GetByTitle(string bookName, int page);
        public Task<Book?> AddAsync(Book newBook);
        public void UpdateAll(Book editedBook);//all Book details
        public void Delete(Book Book);
    }
}
