using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;
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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IBookProxyService _bookProxyService;
        private readonly Mapper _mapper;
        public GenreService(IGenreRepository genreRepository, IBookProxyService bookProxyService)
        {
            _genreRepository = genreRepository;
            _bookProxyService = bookProxyService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<GenreDto>> AddAsync(string genreName)
        {
            var result = await _genreRepository.AddAsync(new Genre { Name = genreName});
            if (result == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "something went wrong while adding your new genre" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto {GenreId=result.Id, GenreName = result.Name } };
        }

        public void Attach(IEnumerable<Genre> genres)
        {
            _genreRepository.Attach(genres);
        }

        public async Task<GenericResultDto<string>> DeleteAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "genre not found" };
            
            _genreRepository.Delete(genre);

            return new GenericResultDto<string> { Succeeded = true, Result = "genre deleted successfully" };
        }

        public void Detach(IEnumerable<Genre> genres)
        {
            _genreRepository.Detach(genres);
        }

        public async Task<GenericResultDto<PagedResult<GenreDto>>> GetAllAsync(int page)
        {
            page = page==0? 1 : page;
            var result = await _genreRepository.GetAllAsync(page);
            var genreDtos = new List<GenreDto>();
            foreach (var item in result)
                genreDtos.Add(new GenreDto { GenreId = item.Id, GenreName = item.Name });
            var finalResult = new PagedResult<GenreDto>
            {
                PageSize = Pagination.Genres,
                Data = genreDtos,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_genreRepository.GetCount(), Pagination.Genres)
            };
            return new GenericResultDto<PagedResult<GenreDto>> { Succeeded = true, Result = finalResult };
        }
        public async Task<GenericResultDto<GenreDto>> GetByIdAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "genre not found" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto {GenreId = genre.Id, GenreName = genre.Name } };
        }
        public async Task<GenericResultDto<List<GenreDto>>> GetByNameAsync(string name)
        {
            var genres = (await _genreRepository.GetByNameAsync(name)).ToList();
            if (genres == null)
                return new GenericResultDto<List<GenreDto>> { Succeeded = false, ErrorMessage = "genre not found" };
            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                genresDto.Add(new GenreDto { GenreId = genre.Id, GenreName = genre.Name });
            }
            
            return new GenericResultDto<List<GenreDto>> { Succeeded = true, Result = genresDto};
        }

        public async Task<GenericResultDto<int>> GetPagesCount()
        {
            var result = (int)Math.Ceiling((double)_genreRepository.GetCount() / Pagination.Genres);
            return new GenericResultDto<int> {Succeeded = true, Result = result};
        }

        public async Task<GenericResultDto<IQueryable<GenreDto>>> GetRangeAsync(HashSet<int> genreIds)
        {
            var genres = await _genreRepository.GetRangeAsync(genreIds);
            if (genres == null)
                return new GenericResultDto<IQueryable<GenreDto>> { Succeeded = false, Result = null};
            return new GenericResultDto<IQueryable<GenreDto>> { Succeeded = true, Result = genres.Select(g => new GenreDto { GenreId = g.Id, GenreName = g.Name }) };
;        }

        public async Task<GenericResultDto<List<BookDto>?>> RandomizeBooksBasedOnGenre(int genreId, int count)
        {
            var books = await _genreRepository.RandomizeBooksBasedOnGenre(genreId, count);
            if (books == null)
                return new GenericResultDto<List<BookDto>?> { Succeeded = false, ErrorMessage = "genre not found" };
            var booksDto = new List<BookDto>();
            foreach (var book in books)
            {
                var dto = _mapper.Map<BookDto>(book);
                dto.Rate = BookHelpers.FormatRate(await _bookProxyService.CalculateRate(book.Id));
                booksDto.Add(dto);
            }

            return new GenericResultDto<List<BookDto>?> { Succeeded = true, Result = booksDto };
        }

        public async Task<GenericResultDto<GenreDto>> UpdateAsync(int currentGenreId, string newGenreName)
        {
            var genre = await _genreRepository.UpdateAsync(currentGenreId, new Genre { Name = newGenreName});
            if (genre == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "something went wrong while updating genre name" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto { GenreId = genre.Id, GenreName = genre.Name } };
        }
    }
}
