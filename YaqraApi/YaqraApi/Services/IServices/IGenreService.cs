﻿using YaqraApi.DTOs;
using YaqraApi.DTOs.Genre;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IGenreService
    {
        Task<GenericResultDto<List<GenreDto>>> GetAllAsync();
        Task<GenericResultDto<GenreDto>> GetByIdAsync(int id);
        Task<GenericResultDto<GenreDto>> GetByNameAsync(string name);
        Task<GenericResultDto<GenreDto>> AddAsync(string genreName);
        Task<GenericResultDto<GenreDto>> UpdateAsync(int currentGenreId, string newGenreName);
        Task<GenericResultDto<string>> DeleteAsync(int id);
    }
}