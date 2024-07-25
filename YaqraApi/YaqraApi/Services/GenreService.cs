using Microsoft.EntityFrameworkCore.Storage.Json;
using YaqraApi.DTOs;
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
        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<GenericResultDto<GenreDto>> AddAsync(string genreName)
        {
            var result = await _genreRepository.AddAsync(new Genre { Name = genreName});
            if (result == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "something went wrong while adding your new genre" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto {GenreId=result.Id, GenreName = result.Name } };
        }
        public async Task<GenericResultDto<string>> DeleteAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "genre not found" };
            
            _genreRepository.Delete(genre);

            return new GenericResultDto<string> { Succeeded = true, Result = "genre deleted successfully" };
        }
        public async Task<GenericResultDto<List<GenreDto>>> GetAllAsync(int page)
        {
            page = page==0? 1 : page;
            var result = await _genreRepository.GetAllAsync(page);
            var genreDtos = new List<GenreDto>();
            foreach (var item in result)
                genreDtos.Add(new GenreDto { GenreId = item.Id, GenreName = item.Name });
            return new GenericResultDto<List<GenreDto>> { Succeeded = true, Result=genreDtos};
        }
        public async Task<GenericResultDto<GenreDto>> GetByIdAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "genre not found" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto {GenreId = genre.Id, GenreName = genre.Name } };
        }
        public async Task<GenericResultDto<GenreDto>> GetByNameAsync(string name)
        {
            var genre = await _genreRepository.GetByNameAsync(name);
            if (genre == null)
                return new GenericResultDto<GenreDto> { Succeeded = false, ErrorMessage = "genre not found" };
            return new GenericResultDto<GenreDto> { Succeeded = true, Result = new GenreDto { GenreId = genre.Id, GenreName = genre.Name } };
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
