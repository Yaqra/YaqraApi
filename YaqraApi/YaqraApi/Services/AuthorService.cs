using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IBookProxyService _bookProxyService;
        private readonly Mapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, 
            IWebHostEnvironment environment,
            IBookProxyService bookProxyService)
        {
            _authorRepository = authorRepository;
            _environment = environment;
            _bookProxyService = bookProxyService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public void Attach(IEnumerable<Author> authors)
        {
            _authorRepository.Attach(authors);
        }
        public async Task<GenericResultDto<AuthorDto?>> AddAsync(IFormFile pic, AuthorDto newAuthor)
        {
            var author = _mapper.Map<Author>(newAuthor);
            var result = await _authorRepository.AddAsync(author);
            if (result == null)
                return new GenericResultDto<AuthorDto?> { Succeeded = false, ErrorMessage = "something went wrong while adding author" };
            var updatePicResult = await UpdatePictureAsync(pic, result.Id);
            if (updatePicResult.Succeeded == true)
                result.Picture = updatePicResult.Result.Picture;
            return new GenericResultDto<AuthorDto?> { Succeeded = true, Result= _mapper.Map<AuthorDto>(result) };
        }

        public async Task<GenericResultDto<List<AuthorDto>>> GetAll(int page)
        {
            page = page == 0 ? 1 : page;
            var authors = (await _authorRepository.GetAll(page)).ToList();
            var authorsDto = _mapper.Map<List<AuthorDto>>(authors);
            authorsDto = await AssignRate(authorsDto);

            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = authorsDto};
        }

        public async Task<GenericResultDto<List<AuthorNameAndIdDto>>> GetAllNamesAndIds(int page)
        {
            page = page == 0 ? 1 : page;
            var authorNamesAndIdsDto = (await _authorRepository.GetAllNamesAndIds(page)).ToList();
            return new GenericResultDto<List<AuthorNameAndIdDto>> { Succeeded = true,Result =authorNamesAndIdsDto};
        }

        public async Task<GenericResultDto<AuthorDto>> GetByIdAsync(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if(author == null)
                return new GenericResultDto<AuthorDto> {Succeeded = false, ErrorMessage = "author not found" };

            var dto = _mapper.Map<AuthorDto>(author);

            dto.Rate = await CalculateAuthorRate(dto.Id);

            return new GenericResultDto<AuthorDto> { Succeeded = true, Result = dto };
        }

        public async Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName, int page)
        {
            page = page == 0 ? 1 : page;
            var authors =( await _authorRepository.GetByName(authorName, page)).ToList();
            if (authors == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "no authors with that name were found" };
            
            var authorsDto = _mapper.Map<List<AuthorDto>>(authors);
            authorsDto = await AssignRate(authorsDto);

            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = authorsDto};
        }

        public async Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId)
        {
            if(pic == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "no picture to add" };

            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "author not found" };

            author.Picture = ImageHelpers.UploadImage(ImageHelpers.AuthorsDir,author.Picture, pic,_environment);

            _authorRepository.UpdateAll(author);

            return new GenericResultDto<AuthorDto> { Succeeded = true, Result = _mapper.Map<AuthorDto>(author) };
        }

        public async Task<GenericResultDto<AuthorDto>> UpdateAllAsync(IFormFile? pic, AuthorWithoutPicDto dto)
        {
            if(pic != null)
                await UpdatePictureAsync(pic, dto.Id);

            var author = await _authorRepository.GetByIdAsync(dto.Id);
            if(author == null) 
                return new GenericResultDto<AuthorDto> { Succeeded= false, ErrorMessage = "author not found" };
            
            if(dto.Name != null)
                author.Name = dto.Name;

            if(dto.Bio != null)
                author.Bio = dto.Bio;

            _authorRepository.UpdateAll(author);

            return new GenericResultDto<AuthorDto> { Succeeded = true, Result = _mapper.Map<AuthorDto>(author) };
        }

        public async Task<GenericResultDto<string>> Delete(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "author not found" };
            _authorRepository.Delete(author);
            return new GenericResultDto<string> { Succeeded = true, Result = "author deleted successfully" };
        }

        public async Task<GenericResultDto<AuthorPagesCount>> GetAuthorsPagesCount()
        {
            var count = _authorRepository.GetCount();
            var result = new AuthorPagesCount
            {
                AuthorsPagesCount = (int)Math.Ceiling((double)count / Pagination.Authors),
                AuthorsNamesAndIdsPagesCount = (int)Math.Ceiling((double)count / Pagination.AuthorNamesAndIds)
            };
            return new GenericResultDto<AuthorPagesCount> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<List<BookDto>>> GetAuthorBooks(int authorId, int page)
        {
            var books = await _authorRepository.GetAuthorBooks(authorId, page);
            var result = BookHelpers.ConvertBooksToBookDtos(books);
            return new GenericResultDto<List<BookDto>>
            {
                Succeeded = true,
                Result = result.ToList()
            };
        }
        private async Task<GenericResultDto<List<int>>> GetAuthorBooksIds(int authorId)
        {
            var booksIds = await _authorRepository.GetAuthorBooksIds(authorId);
            if (booksIds == null)
                return new GenericResultDto<List<int>> { Succeeded = false, ErrorMessage = "author not found" };

            return new GenericResultDto<List<int>> { Succeeded = true, Result = booksIds };
        }
        private async Task<string?> CalculateAuthorRate(int authorId)
        {
            var booksIdsResult = await GetAuthorBooksIds(authorId);//there is a problem here attaching author entity to efcore
            if (booksIdsResult.Succeeded == false)
                return null;

            List<decimal> booksRates = new();
            foreach (var bookId in booksIdsResult.Result)
            {
                var bookRate = await _bookProxyService.CalculateRate(bookId);
                if(bookRate != null)
                    booksRates.Add(bookRate.Value);
            }
            var authorRate = booksRates.Sum() / booksRates.Count();

            return BookHelpers.FormatRate(authorRate);
        }
        private async Task<List<AuthorDto>> AssignRate(List<AuthorDto> authorsDto)
        {
            foreach (var dto in authorsDto)
                dto.Rate = await CalculateAuthorRate(dto.Id);
            return authorsDto;
        }

        public async Task<GenericResultDto<IQueryable<AuthorDto>>> GetRangeAsync(HashSet<int> authorsIds)
        {
            var authors = await _authorRepository.GetRangeAsync(authorsIds);
            if (authors == null)
                return new GenericResultDto<IQueryable<AuthorDto>> { Succeeded = false, Result = null };

            var authorsDto = authors.ProjectTo<AuthorDto>(_mapper.ConfigurationProvider);

            return new GenericResultDto<IQueryable<AuthorDto>> { Succeeded = true, Result = authorsDto };
        }
    }
}
