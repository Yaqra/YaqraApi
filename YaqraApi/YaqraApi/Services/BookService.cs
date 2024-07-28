using AutoMapper;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Genre;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

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
            IAuthorService authorService, IWebHostEnvironment environment)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _authorService = authorService;
            _environment = environment;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<BookDto?>> AddAsync(AddBookDto dto)
        {
            var book = new Book
            {
                AddedDate = DateTime.UtcNow,
                Description = dto.Description,
                NumberOfPages = dto.NumberOfPages,
                Title = dto.Title,
            };
            
            var result = await _bookRepository.AddAsync(book);
            if (result == null)
                return new GenericResultDto<BookDto?> { Succeeded = false, ErrorMessage = "something went wrong while adding book" };

            if (dto.GenresIds != null)
            {
                book.Genres = new List<Genre>();
                foreach (var id in dto.GenresIds)
                {
                    var genre = (await _genreService.GetByIdAsync(id)).Result;
                    if (genre == null)
                        continue;
                    book.Genres.Add(new Genre { Id = genre.GenreId, Name = genre.GenreName });
                }
            }

            book.Authors = new List<Author>();
            foreach (var id in dto.AuthorsIds)
            {
                var author = (await _authorService.GetByIdAsync(id)).Result;
                if (author == null)
                    continue;
                book.Authors.Add(_mapper.Map<Author>(author));
            }

            _bookRepository.UpdateAll(book);

            if(dto.Image != null) 
            { 
                var updateImgResult = await UpdateImageAsync(dto.Image, result.Id);
                if (updateImgResult.Succeeded == true)
                    result.Image = updateImgResult.Result.Image;
            }

            return new GenericResultDto<BookDto?> { Succeeded = true, Result = _mapper.Map<BookDto>(result) };
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
                result.Add(_mapper.Map<BookDto>(book));
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
            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
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

            var oldImgPath = book.Image;

            var imgName = Path.GetFileName(img.FileName);
            var imgExtension = Path.GetExtension(imgName);
            var imgWithGuid = $"{imgName.TrimEnd(imgExtension.ToArray())}{Guid.NewGuid().ToString()}{imgExtension}";
            var dir = Path.Combine(_environment.WebRootPath, "Books");
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);
            var imgPath = Path.Combine(dir, imgWithGuid);

            var createImg = Task.Run(async () =>
            {
                using (var stream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
                {
                    await img.CopyToAsync(stream);
                    book.Image = $"/Books/{imgWithGuid}";
                }
            });
            var deleteOldImg = Task.Run(() =>
            {
                if (string.IsNullOrEmpty(oldImgPath) == false && File.Exists(oldImgPath))
                    File.Delete(oldImgPath);
            });
            Task.WaitAll(createImg, deleteOldImg);

            _bookRepository.UpdateAll(book);

            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
        }
    }
}
