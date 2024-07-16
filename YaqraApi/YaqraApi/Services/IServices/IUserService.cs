using System.Security.Claims;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Genre;
using YaqraApi.DTOs.User;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface IUserService
    {
        Task<GenericResultDto<ApplicationUser>> UpdateBioAsync(string bio, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateUsernameAsync(string username, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdatePasswordAsync(PasswordUpdateDto dto, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId);
        Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId);
        Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId);
        Task<GenericResultDto<UserDto>> GetUserAsync(string userId);
        GenericResultDto<List<UsernameAndId>> GetUserFollowersNames(string userId);
        GenericResultDto<List<UsernameAndId>> GetUserFollowingsNames(string userId);
        Task<GenericResultDto<List<GenreDto>>> AddFavouriteGenresAsync(List<GenreIdDto> genres, string userId);
        Task<GenericResultDto<List<GenreDto>>> GetFavouriteGenresAsync(string userId);
        Task<GenericResultDto<List<GenreDto>>> GetAllGenresExceptUserGenresAsync(string userId);
        Task<GenericResultDto<List<GenreDto>>> DeleteFavouriteGenresAsync(GenreIdDto genre, string userId);

    }
}
