using AutoMapper;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;
using YaqraApi.Repositories;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly Mapper _mapper;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<BookDto?>> AddAsync(IFormFile img, BookDto newBook)
        {
            var book = _mapper.Map<Book>(newBook);
            var result = await _bookRepository.AddAsync(book);
            if (result == null)
                return new GenericResultDto<BookDto?> { Succeeded = false, ErrorMessage = "something went wrong while adding book" };
            var updateImgResult = await UpdateImageAsync(img, result.Id);
            if (updateImgResult.Succeeded == true)
                result.Image = updateImgResult.Result.Image;
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

        public async Task<GenericResultDto<List<BookDto>>> GetAll()
        {
            var books = await _bookRepository.GetAll();
            var result = new List<BookDto>();
            foreach (var book in books)
                result.Add(_mapper.Map<BookDto>(book));
            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookTitleAndIdDto>>> GetAllTitlesAndIds()
        {
            var bookTitlesAndIdsDto = (await _bookRepository.GetAllTitlesAndIds()).ToList();
            return new GenericResultDto<List<BookTitleAndIdDto>> { Succeeded = true, Result = bookTitlesAndIdsDto };
        }

        public async Task<GenericResultDto<BookDto>> GetByIdAsync(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };
            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(book) };
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

            var createImg = Task.Run(async () =>
            {
                var imgName = Path.GetFileName(img.FileName);
                var imgExtension = Path.GetExtension(imgName);
                var imgWithGuid = $"{imgName.TrimEnd(imgExtension.ToArray())}{Guid.NewGuid().ToString()}{imgExtension}";

                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Books", imgWithGuid);

                using (var stream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
                {
                    await img.CopyToAsync(stream);
                    book.Image = imgPath;
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
