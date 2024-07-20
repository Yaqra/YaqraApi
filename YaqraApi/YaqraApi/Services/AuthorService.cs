using AutoMapper;
using Microsoft.AspNetCore.Identity;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly Mapper _mapper;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
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

        public async Task<GenericResultDto<List<AuthorDto>>> GetAll()
        {
            var authors = await _authorRepository.GetAll();
            var result = new List<AuthorDto>();
            foreach (var author in authors)
                result.Add(_mapper.Map<AuthorDto>(author));
            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result=result};
        }

        public async Task<GenericResultDto<List<AuthorNameAndIdDto>>> GetAllNamesAndIds()
        {
            var authorNamesAndIdsDto = (await _authorRepository.GetAllNamesAndIds()).ToList();
            return new GenericResultDto<List<AuthorNameAndIdDto>> { Succeeded = true,Result =authorNamesAndIdsDto};
        }

        public async Task<GenericResultDto<AuthorDto>> GetByIdAsync(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if(author == null)
                return new GenericResultDto<AuthorDto> {Succeeded = false, ErrorMessage = "author not found" };
            return new GenericResultDto<AuthorDto> { Succeeded = true, Result = _mapper.Map<AuthorDto>(author) };
        }

        public async Task<GenericResultDto<List<AuthorDto>>> GetByName(string authorName)
        {
            var authors = await _authorRepository.GetByName(authorName);
            if (authors == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "no authors with that name were found" };
            
            var authorsDto = new List<AuthorDto>();
            foreach(var author in authors)
                authorsDto.Add(_mapper.Map<AuthorDto>(author));
            
            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = authorsDto};
        }

        public async Task<GenericResultDto<AuthorDto>> UpdatePictureAsync(IFormFile pic, int authorId)
        {
            if(pic == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "no picture to add" };

            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
                return new GenericResultDto<AuthorDto> { Succeeded = false, ErrorMessage = "author not found" };

            var oldPicPath = author.Picture;

            var createPic = Task.Run(async () =>
            {
                var picName = Path.GetFileName(pic.FileName);
                var picExtension = Path.GetExtension(picName);
                var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";

                var picPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Authors", picWithGuid);

                using (var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    author.Picture = picPath;
                }
            });
            var deleteOldPic = Task.Run(() =>
            {
                if (string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

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
    }
}
