using AutoMapper;
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
        private readonly Mapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, 
            IWebHostEnvironment environment)
        {
            _authorRepository = authorRepository;
            _environment = environment;
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
            var result = new List<AuthorDto>();
            foreach (var author in authors)
            {
                var dto = _mapper.Map<AuthorDto>(author);
                dto.Rate = await CalculateAuthorRate(author.Id);
                result.Add(dto);
            }
            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result=result};
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

            dto.Rate = await CalculateAuthorRate(dto.Id);//there is a problem here attaching author entity to efcore

            return new GenericResultDto<AuthorDto> { Succeeded = true, Result = dto };
        }

        public async Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName, int page)
        {
            page = page == 0 ? 1 : page;
            var authors =( await _authorRepository.GetByName(authorName, page)).ToList();
            if (authors == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "no authors with that name were found" };
            
            var authorsDto = new List<AuthorDto>();
            foreach(var author in authors)
            {
                var dto = _mapper.Map<AuthorDto>(author);
                dto.Rate = await CalculateAuthorRate(author.Id);
                authorsDto.Add(dto);
            }

            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = authorsDto};
        }

        public async Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId)
        {
            if(pic == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "no picture to add" };

            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "author not found" };

            //var oldPicPath = author.Picture;

            //var picName = Path.GetFileName(pic.FileName);
            //var picExtension = Path.GetExtension(picName);
            //var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";
            //var dir= Path.Combine(_environment.WebRootPath, "Authors");
            //if (Directory.Exists(dir)==false)
            //    Directory.CreateDirectory(dir);
            //var picPath = Path.Combine(dir, picWithGuid);

            //var createPic = Task.Run(async () =>
            //{
            //    using (var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
            //    {
            //        await pic.CopyToAsync(stream);
            //        author.Picture = $"/Authors/{picWithGuid}";
            //    }

            //});
            //var deleteOldPic = Task.Run(() =>
            //{
            //    if (string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
            //        File.Delete(oldPicPath);
            //});
            //Task.WaitAll(createPic, deleteOldPic);

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

            var booksRates = await _authorRepository.GetAuthorBooksRates(booksIdsResult.Result);

            return BookHelpers.CalcualteRate(booksRates);
        }
    }
}
