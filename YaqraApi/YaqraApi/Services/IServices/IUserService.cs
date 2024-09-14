﻿using System.Security.Claims;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Genre;
using YaqraApi.DTOs.ReadingGoal;
using YaqraApi.DTOs.User;
using YaqraApi.DTOs.UserBookWithStatus;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

namespace YaqraApi.Services.IServices
{
    public interface IUserService
    {
        Task<GenericResultDto<ApplicationUser>> UpdateAllAsync(IFormFile? pic, IFormFile? cover, UserDto dto);
        Task<GenericResultDto<ApplicationUser>> UpdatePasswordAsync(PasswordUpdateDto dto, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId);
        Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId);
        Task<GenericResultDto<UserDto>> GetUserAsync(string userId, string followerId);
        GenericResultDto<List<UserNameAndId>> GetUserFollowersNames(string userId, int page);
        GenericResultDto<int> GetUserFollowersPagesCount(string userId);
        GenericResultDto<List<UserNameAndId>> GetUserFollowingsNames(string userId, int page);
        GenericResultDto<int> GetUserFollowingsPagesCount(string userId);
        Task<GenericResultDto<List<GenreDto>>> AddFavouriteGenresAsync(List<GenreIdDto> genres, string userId);
        Task<GenericResultDto<List<GenreDto>>> GetFavouriteGenresAsync(string userId);
        Task<GenericResultDto<List<GenreDto>>> GetAllGenresExceptUserGenresAsync(string userId, int page);
        Task<GenericResultDto<int>> GetGenresExceptUserGenresPagesCountAsync(string userId);
        Task<GenericResultDto<string>> DeleteFavouriteGenreAsync(GenreIdDto genre, string userId);
        Task<GenericResultDto<List<AuthorDto>>> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors, string userId, int page);
        Task<GenericResultDto<int>> GetFavouriteAuthorsPagesCountAsync(string userId);
        Task<GenericResultDto<List<AuthorDto>>> GetFavouriteAuthorsAsync(string userId, int page);
        Task<GenericResultDto<int>> GetFavouriteAuthorsExceptUserPagesCountAsync(string userId);
        Task<GenericResultDto<List<AuthorDto>>> GetAllAuthorsExceptUserAuthorsAsync(string userId, int page);
        Task<GenericResultDto<string>> DeleteFavouriteAuthorAsync(AuthorIdDto author, string userId);
        Task<GenericResultDto<ReadingGoalDto>> AddReadingGoalAsync(ReadingGoalDto dto, string userId);
        Task<GenericResultDto<List<ReadingGoalDto>>> GetAllReadingGoalsAsync(string userId, int page);
        Task<GenericResultDto<int>> GetReadingGoalsPagesCountAsync(string userId);
        Task<GenericResultDto<string>> DeleteReadingGoalAsync(int goalId, string userId);
        Task<GenericResultDto<ReadingGoalDto>> UpdateReadingGoalAsync(UpdateReadingGoalDto dto, string userId);
        Task<GenericResultDto<BookDto>> AddBookToCollectionAsync(UserBookWithStatusDto dto, string userId);
        Task<GenericResultDto<List<BookDto>>> GetBooksAsync(int? status, string userId, int page);
        Task<GenericResultDto<BookCollectionPages>> GetBooksPagesCountAsync(string userId);
        Task<GenericResultDto<BookDto>> UpdateBookStatusAsync(int bookId, int? status, string userId);
        Task<GenericResultDto<string>> DeleteBookAsync(int bookId, string userId);
        Task<GenericResultDto<string>> AddConnectionIdToUser(string userId, string connectionId);
        Task<GenericResultDto<string>> RemoveConnectionIdFromUser(string userId, string connectionId);
        Task<List<string>?> GetUserConnections(string userId);
    }
}
