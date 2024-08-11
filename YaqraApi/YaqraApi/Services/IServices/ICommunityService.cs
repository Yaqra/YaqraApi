using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

namespace YaqraApi.Services.IServices
{
    public interface ICommunityService
    {
        Task<GenericResultDto<ReviewDto>> AddReviewAsync(AddReviewDto review);
        Task<GenericResultDto<ReviewDto>> GetReviewAsync(int reviewId);
        Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(UpdateReviewDto editedReview);
        Task<GenericResultDto<List<ReviewDto>>> GetAllReviewsAsync(int page);
        Task<GenericResultDto<PlaylistDto>> AddPlaylistAsync(AddPlaylistDto playlist);
        Task<GenericResultDto<PlaylistDto>> GetPlaylistAsync(int playlistId);
        Task<GenericResultDto<PlaylistDto>> UpdatePlaylistAsync(UpdatePlaylistDto editedPlaylist);
        Task<GenericResultDto<PlaylistDto>> AddBooksToPlaylist(int playlistId, List<int>booksIds);
        Task<GenericResultDto<PlaylistDto>> RemoveBooksFromPlaylist(int playlistId, List<int> booksIds);
        Task<GenericResultDto<List<PlaylistDto>>> GetAllPlaylistsAsync(int page);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddDiscussionAsync(AddDiscussionArticleNewsDto discussion);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> GetDiscussionAsync(int discussionId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> UpdateDiscussionAsync(UpdateDiscussionArticleNewsDto editedDiscussion);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddBooksToDiscussion(int discussionId, List<int> booksIds);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> RemoveBooksFromDiscussion(int discussionId, List<int> booksIds);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag);
        Task LikeAsync(int postId);
        Task<GenericResultDto<string>> Delete(int postId);
        Task<GenericResultDto<List<ReviewDto>>> GetUserReviews(string userId, int page);
        Task<GenericResultDto<List<PlaylistDto>>> GetUserPlaylists(string userId, int page);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetUserDiscussions(string userId, int page);
    }
}
