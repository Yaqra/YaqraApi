
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
        private readonly IBookProxyService _bookProxyService;
        private readonly Mapper _mapper;

        public BookService(
            IBookRepository bookRepository, 
            IGenreService genreService,
            IAuthorService authorService, 
            IWebHostEnvironment environment,
            IBookProxyService bookProxyService)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _authorService = authorService;
            _environment = environment;
            _bookProxyService = bookProxyService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public void Attach(IEnumerable<Book> books)
        {
            _bookRepository.Attach(books);
        }

        public async Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto)
        {
            var book = await CreateBook(dto);

            if (dto.GenresIds != null)
                book = await AddGenresToBook(dto.GenresIds, book);

            book = await AddAuthorsToBook(dto.AuthorsIds, book);

            await _bookRepository.AddAsync(book);

            if (dto.Image != null)
            {
                var updateImgResult = await UpdateImageAsync(dto.Image, book.Id);
                if (updateImgResult.Succeeded == true)
                    book.Image = updateImgResult.Result.Image;
            }
            var bookDto = _mapper.Map<BookDto>((await GetByIdAsync(book.Id)).Result);

            return new GenericResultDto<BookDto?> { Succeeded = true, Result = bookDto };
        }

        public async Task<GenericResultDto<string>> Delete(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "book not found" };
            _bookRepository.Delete(book);
            return new GenericResultDto<string> { Succeeded = true, Result = "book deleted successfully" };
        }

        public async Task<GenericResultDto<PagedResult<BookDto>>> GetAll(int page)
        {
            page = page == 0 ? 1 : page;
            var books = await _bookRepository.GetAll(page);
            var booksDto = new List<BookDto>();
            foreach (var book in books)
            {
                var dto = _mapper.Map<BookDto>(book);
                
                dto.GenresDto = book.Genres.Select(genre=> new GenreDto { GenreId = genre.Id, GenreName = genre.Name }).ToList();

                dto.AuthorsDto = _mapper.Map<List<AuthorDto>>(book.Authors);

                dto.Rate = BookHelpers.FormatRate(await _bookProxyService.CalculateRate(dto.Id));

                booksDto.Add(dto);
            }

            var result = new PagedResult<BookDto>
            {
                PageSize = Pagination.Books,
                Data = booksDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_bookRepository.GetCount(), Pagination.Books)
            };

            return new GenericResultDto<PagedResult<BookDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<PagedResult<BookTitleAndIdDto>>> GetAllTitlesAndIds(int page)
        {
            page = page==0? 1 : page;
            var bookTitlesAndIdsDto = (await _bookRepository.GetAllTitlesAndIds(page)).ToList();

            var result = new PagedResult<BookTitleAndIdDto>
            {
                PageSize = Pagination.BookTitlesAndIds,
                Data = bookTitlesAndIdsDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_bookRepository.GetCount(), Pagination.BookTitlesAndIds)
            };

            return new GenericResultDto<PagedResult<BookTitleAndIdDto>> { Succeeded = true, Result = result};
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

            result.AuthorsDto = _mapper.Map<List<AuthorDto>>(book.Authors);

            result.GenresDto = book.Genres.Select(genre => new GenreDto { GenreId = genre.Id, GenreName = genre.Name }).ToList();

            result.Rate = BookHelpers.FormatRate(await _bookProxyService.CalculateRate(bookId));

            return new GenericResultDto<BookDto> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookDto>>> GetByTitle(string BookName)
        {
            var books = await _bookRepository.GetByTitle(BookName);
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
            book.NumberOfPages = dto.NumberOfPages;
            book.Description = dto.Description;
            book.Title = dto.Title;

            _bookRepository.UpdateAll(book);

            return await GetByIdAsync(book.Id);
        }

        public async Task<GenericResultDto<BookDto>> UpdateImageAsync(IFormFile img, int bookId)
        {
            if (img == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "no image to add" };

            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };

            book.Authors = null;
            book.Genres = null;
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

            return book;
        }
        private async Task<Book> AddGenresToBook(HashSet<int> genresIds, Book book)
        {
            if (book.Genres != null)
            {
                foreach (var genre in book.Genres)
                {
                    if (genresIds.Contains(genre.Id))
                        genresIds.Remove(genre.Id);
                }
            }
            if (genresIds.IsNullOrEmpty())
                return book;
            var genres = (await _genreService.GetRangeAsync(genresIds)).Result.ToList();
            if (genres == null)
                return book;
            book.Genres = genres.Select(g => new Genre { Id = g.GenreId, Name = g.GenreName }).ToList();
            _genreService.Attach(book.Genres);
            return book;
        }
        private async Task<Book> AddAuthorsToBook(HashSet<int> AuthorsIds, Book book)
        {
            if(book.Authors != null)
            {
                foreach (var author in book.Authors)
                {
                    if (AuthorsIds.Contains(author.Id))
                        AuthorsIds.Remove(author.Id);
                }
            }
            if (AuthorsIds.IsNullOrEmpty())
                return book;
            var authors = (await _authorService.GetRangeAsync(AuthorsIds)).Result.ToList();
            if (authors == null)
                return book;
            book.Authors = _mapper.Map<List<Author>>(authors);
            _authorService.Attach(book.Authors);
            return book;
        }
        public async Task<GenericResultDto<PagedResult<BookDto>>> GetRecent(int page)
        {
            page = page == 0 ? 1 : page;

            var books = await _bookRepository.GetRecent(page);

            var booksDto = BookHelpers.ConvertBooksToBookDtos(books.ToList());

            var result = new PagedResult<BookDto>
            {
                PageSize = Pagination.Books,
                Data = booksDto.ToList(),
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_bookRepository.GetRecentCount(), Pagination.Books)
            };

            return new GenericResultDto<PagedResult<BookDto>> { Succeeded = true, Result = result};
        }

        public async Task<GenericResultDto<BookDto>> AddGenresToBook(HashSet<int> genresIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Authors = null;
            var firstGenre = book.Genres.FirstOrDefault();
            book = await AddGenresToBook(genresIds, book);
            if (book.Genres.FirstOrDefault() == firstGenre)
            {
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "genres already exist" };
            }
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> RemoveGenresFromBook(HashSet<int> genresIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Authors = null;
            var genresToRemove = book.Genres.Where(g => genresIds.Contains(g.Id));

            var genresCount = book.Genres.Count;
            _genreService.Attach(genresToRemove);
            foreach (var genre in genresToRemove)
                book.Genres.Remove(genre);

            if (book.Genres.Count == genresCount)
            {
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "no genres to remove" };
            }
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> AddAuthorsToBook(HashSet<int> AuthorsIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Genres = null;
            var firstAuthor= book.Authors.FirstOrDefault();
            book = await AddAuthorsToBook(AuthorsIds, book);
            if (book.Authors.FirstOrDefault() == firstAuthor)
            {
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "authors already exist" };
            }
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<BookDto>> RemoveAuthorsFromBook(HashSet<int> authorIds, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            book.Genres = null;
            var authorsToRemove = book.Authors.Where(g => authorIds.Contains(g.Id));

            var authorsCount = book.Authors.Count;
            _authorService.Attach(authorsToRemove);
            foreach (var author in authorsToRemove)
                book.Authors.Remove(author);

            if (book.Authors.Count == authorsCount)
            {
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "no authors to remove" };
            }
            _bookRepository.UpdateAll(book);
            var result = await GetByIdAsync(bookId);
            return result;
        }

        public async Task<GenericResultDto<PagedResult<ReviewDto>>> GetReviews(int bookId, int page, SortType type, ReviewsSortField field)
        {
            page = page == 0 ? 1 : page;
            var reviews = await _bookRepository.GetReviews(bookId, page, type, field);
            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);

            var result = new PagedResult<ReviewDto>
            {
                PageSize = Pagination.Posts,
                Data = reviewsDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_bookRepository.GetReviewsCount(bookId), Pagination.Posts)
            };

            return new GenericResultDto<PagedResult<ReviewDto>> { Succeeded = true, Result = result };
    
        }

        public async Task<GenericResultDto<PagedResult<BookDto>>> FindBooks(BookFinderDto dto)
        {
            dto.Page = dto.Page == 0 ? 1 : dto.Page;

            var booksDto = await _bookRepository.FindBooks(dto, _bookProxyService);

            var result = new PagedResult<BookDto>
            {
                PageSize = Pagination.Books,
                Data = booksDto,
                PageNumber = dto.Page,
                TotalPages = Pagination.CalculatePagesCount(await _bookRepository.FindBooksCount(dto, _bookProxyService), Pagination.Books)
            };

            return new GenericResultDto<PagedResult<BookDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookDto>>> GetTrendingBooks()
        {
            var books = await _bookRepository.GetTrendingBooks();
            if (books == null)
                return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = new List<BookDto>()};

            var booksDto = new List<BookDto>();
            foreach (var book in books)
            {
                var dto = _mapper.Map<BookDto>(book);
                dto.Rate = BookHelpers.FormatRate(await _bookProxyService.CalculateRate(book.Id));
                booksDto.Add(dto);
            }
            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = booksDto};
        }

        public async Task AddTrendingBook(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return;
            await _bookRepository.AddTrendingBook(new TrendingBook { BookId = bookId });
        }

        public async Task<GenericResultDto<PagedResult<BookDto>>> GetUpcomingBooks(int page)
        {
            page = page == 0 ? 1 : page;
            var books = await _bookRepository.GetUpcomingBooks(page);
            var booksDto = _mapper.Map<List<BookDto>>(books);

            var result = new PagedResult<BookDto>
            {
                PageSize = Pagination.Books,
                Data = booksDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_bookRepository.GetUpcomingBooksCount(), Pagination.Books)
            };

            return new GenericResultDto<PagedResult<BookDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<IQueryable<Book>>> GetRangeAsync(HashSet<int> booksIds)
        {
            return new GenericResultDto<IQueryable<Book>> { Succeeded = true, Result = await _bookRepository.GetRange(booksIds) };
        }

        public async Task LoadGenres(Book book)
        {
            await _bookRepository.LoadGenres(book);
        }

        public bool IsEntityTracked(Book book)
        {
            return _bookRepository.IsEntityTracked(book);
        }

    }
}
