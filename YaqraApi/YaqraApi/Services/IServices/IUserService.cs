using System.Security.Claims;
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
        Task<GenericResultDto<UserDto>> GetUserAsync(string userId);
        GenericResultDto<List<UserNameAndId>> GetUserFollowersNames(string userId);
        GenericResultDto<List<UserNameAndId>> GetUserFollowingsNames(string userId);
        Task<GenericResultDto<List<GenreDto>>> AddFavouriteGenresAsync(List<GenreIdDto> genres, string userId);
        Task<GenericResultDto<List<GenreDto>>> GetFavouriteGenresAsync(string userId);
        Task<GenericResultDto<List<GenreDto>>> GetAllGenresExceptUserGenresAsync(string userId);
        Task<GenericResultDto<List<GenreDto>>> DeleteFavouriteGenreAsync(GenreIdDto genre, string userId);
        Task<GenericResultDto<List<AuthorDto>>> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors, string userId);
        Task<GenericResultDto<List<AuthorDto>>> GetFavouriteAuthorsAsync(string userId);
        Task<GenericResultDto<List<AuthorDto>>> GetAllAuthorsExceptUserAuthorsAsync(string userId);
        Task<GenericResultDto<List<AuthorDto>>> DeleteFavouriteAuthorAsync(AuthorIdDto author, string userId);
        Task<GenericResultDto<List<ReadingGoalDto>>> AddReadingGoalAsync(ReadingGoalDto dto, string userId);
        Task<GenericResultDto<List<ReadingGoalDto>>> GetAllReadingGoalsAsync(string userId);
        Task<GenericResultDto<List<ReadingGoalDto>>> DeleteReadingGoalAsync(int goalId, string userId);
        Task<GenericResultDto<List<ReadingGoalDto>>> UpdateReadingGoalAsync(UpdateReadingGoalDto dto, string userId);
        Task<GenericResultDto<List<BookDto>>> AddBookToCollectionAsync(UserBookWithStatusDto dto, string userId);
        Task<GenericResultDto<List<BookDto>>> GetBooksAsync(int? status, string userId);
        Task<GenericResultDto<List<BookDto>>> UpdateBookStatusAsync(int bookId, int? status, string userId);
        Task<GenericResultDto<string>> DeleteBookAsync(int bookId, string userId);

    }
}
