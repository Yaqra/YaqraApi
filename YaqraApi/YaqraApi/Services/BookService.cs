
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Community;
using YaqraApi.DTOs.Genre;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;
using static System.Net.Mime.MediaTypeNames;

namespace YaqraApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _environment;
        private readonly Mapper _mapper;

        public BookService(
            IBookRepository bookRepository, 
            IGenreService genreService,
            IAuthorService authorService, 
            IWebHostEnvironment environment)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _authorService = authorService;
            _environment = environment;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public void Attach(IEnumerable<Book> books)
        {
            _bookRepository.Attach(books);
        }
        public async Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto)
        {
            var book = await CreateBook(dto);
            if (book == null)
                return new GenericResultDto<BookDto?> { Succeeded = false, ErrorMessage = "something went wrong while adding book" };

            if (dto.GenresIds != null)
                book = await AddGenresToBook(dto.GenresIds, book);

            book = await AddAuthorsToBook(dto.AuthorsIds, book);

            _bookRepository.UpdateAll(book);

            if(dto.Image != null) 
            { 
                var updateImgResult = await UpdateImageAsync(dto.Image, book.Id);
                if (updateImgResult.Succeeded == true)
                    book.Image = updateImgResult.Result.Image;
            }

            return new GenericResultDto<BookDto?> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
        }

        public async Task<GenericResultDto<string>> Delete(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "book not found" };
            _bookRepository.Delete(book);
            return new GenericResultDto<string> { Succeeded = true, Result = "book deleted successfully" };
        }

        public async Task<GenericResultDto<List<BookDto>>> GetAll(int page)
        {
            page = page == 0 ? 1 : page;
            var books = await _bookRepository.GetAll(page);
            var result = new List<BookDto>();
            foreach (var book in books)
            {
                var dto = _mapper.Map<BookDto>(book);
                var x = await _bookRepository.GetBookRates(dto.Id);
                foreach(var genre in book.Genres)
                    dto.GenresDto.Add(new GenreDto { GenreId = genre.Id, GenreName = genre.Name });
                foreach (var author in book.Authors)
                    dto.AuthorsDto.Add(_mapper.Map<AuthorDto>(author));

                    dto.Rate = BookHelpers.CalcualteRate(x);
                result.Add(dto);
            }

            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookTitleAndIdDto>>> GetAllTitlesAndIds(int page)
        {
            page = page==0? 1 : page;
            var bookTitlesAndIdsDto = (await _bookRepository.GetAllTitlesAndIds(page)).ToList();
            return new GenericResultDto<List<BookTitleAndIdDto>> { Succeeded = true, Result = bookTitlesAndIdsDto };
        }

        public async Task<GenericResultDto<BookPagesCount>> GetBooksPagesCount()
        {
            var count = _bookRepository.GetCount();
            var result = new BookPagesCount
            {
                BooksPagesCount = (int)Math.Ceiling((double)count / Pagination.Books),
                BooksTitlesAndIdsPagesCount = (int)Math.Ceiling((double)count / Pagination.BookTitlesAndIds)
            };
            return new GenericResultDto<BookPagesCount> {Succeeded= true, Result= result };
        }     
        public async Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };

            var result = _mapper.Map<BookDto>(book);

            foreach (var author in book.Authors)
                result.AuthorsDto.Add(_mapper.Map<AuthorDto>(author));

            foreach (var genre in book.Genres)
                result.GenresDto.Add(new GenreDto { GenreId = genre.Id, GenreName = genre.Name });

            result.Rate = BookHelpers.CalcualteRate(await _bookRepository.GetBookRates(bookId));

            return new GenericResultDto<BookDto> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookDto>>> GetByTitle(string BookName, int page)
        {
            page = page == 0 ? 1 : page;
            var books = await _bookRepository.GetByTitle(BookName, page);
            if (books == null)
                return new GenericResultDto<List<BookDto>> { Succeeded = false, ErrorMessage = "no books with that title were found" };

            var booksDto = new List<BookDto>();
            foreach (var book in books)
                booksDto.Add(_mapper.Map<BookDto>(book));

            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = booksDto };
        }

        public async Task<GenericResultDto<BookDto>> UpdateAllAsync(IFormFile? img, BookWithoutImageDto dto)
        {
            if (img != null)
                await UpdateImageAsync(img, dto.Id);

            var book = await _bookRepository.GetByIdAsync(dto.Id);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            
            book.Authors = null;
            book.Genres = null;

            if (dto.NumberOfPages != null)
                book.NumberOfPages = dto.NumberOfPages;

            if (dto.Description != null)
                book.Description = dto.Description;

            if (dto.Title != null)
                book.Title = dto.Title;

            _bookRepository.UpdateAll(book);

            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
        }

        public async Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId)
        {
            if (img == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "no image to add" };

            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };

            book.Image= ImageHelpers.UploadImage(ImageHelpers.BooksDir, book.Image, img, _environment);

            _bookRepository.UpdateAll(book);

            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
        }
        private async Task<Book?> CreateBook(AddBookDto dto)
        {
            var book = new Book
            {
                AddedDate = DateTime.UtcNow,
                Description = dto.Description,
                NumberOfPages = dto.NumberOfPages,
                Title = dto.Title,
            };

            return await _bookRepository.AddAsync(book);
        }
        private async Task<Book> AddGenresToBook(List<int> genresIds, Book book)
        {
            book.Genres = new List<Genre>();
            foreach (var id in genresIds)
            {
                var genre = (await _genreService.GetByIdAsync(id)).Result;
                if (genre == null)
                    continue;
                book.Genres.Add(new Genre { Id = genre.GenreId, Name = genre.GenreName });
            }
            return book;
        }
        private async Task<Book> AddAuthorsToBook(List<int> AuthorsIds, Book book)
        {
            book.Authors = new List<Author>();
            foreach (var id in AuthorsIds)
            {
                var author = (await _authorService.GetByIdAsync(id)).Result;
                if (author == null)
                    continue;
                book.Authors.Add(_mapper.Map<Author>(author));
            }
            return book;
        }
        public async Task<GenericResultDto<List<BookDto>>> GetRecent(int page)
        {
            var books = await _bookRepository.GetRecent(page);

            var result = BookHelpers.ConvertBooksToBookDtos(books.ToList());

            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = result.ToList()};
        }

        public async Task<GenericResultDto<BookDto>> AddGenresToBook(List<int> genresIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Authors = null;
            book = await AddGenresToBook(genresIds, book);
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> RemoveGenresFromBook(List<int> genresIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Authors = null;
            var genresToRemove = book.Genres.Where(g => genresIds.Contains(g.Id));

            _genreService.Attach(genresToRemove);
            foreach (var genre in genresToRemove)
                book.Genres.Remove(genre);

            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> AddAuthorsToBook(List<int> AuthorsIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Genres = null;
            book = await AddAuthorsToBook(AuthorsIds, book);
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> RemoveAuthorsFromBook(List<int> authorIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Genres = null;
            var authorsToRemove = book.Authors.Where(g => authorIds.Contains(g.Id));

            _authorService.Attach(authorsToRemove);
            foreach (var author in authorsToRemove)
                book.Authors.Remove(author);

            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<List<ReviewDto>>> GetReviews(int bookId, int page, SortType type, ReviewsSortField field)
        {
            page = page == 0 ? 1 : page;
            var reviews = await _bookRepository.GetReviews(bookId, page, type, field);
            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
                reviewsDto.Add(_mapper.Map<ReviewDto>(review));

            return new GenericResultDto<List<ReviewDto>> { Succeeded = true, Result = reviewsDto };
    
        }
    }
}
