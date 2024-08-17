using System.Collections;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

namespace YaqraApi.Services.IServices
{
    public interface ICommunityService
    {
        Task<GenericResultDto<ReviewDto>> AddReviewAsync(AddReviewDto review, string userId);
        Task<GenericResultDto<ReviewDto>> GetReviewAsync(int reviewId);
        Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(UpdateReviewDto editedReview);
        Task<GenericResultDto<List<ReviewDto>>> GetAllReviewsAsync(int page);
        Task<GenericResultDto<PlaylistDto>> AddPlaylistAsync(AddPlaylistDto playlist);
        Task<GenericResultDto<PlaylistDto>> GetPlaylistAsync(int playlistId);
        Task<GenericResultDto<PlaylistDto>> UpdatePlaylistAsync(UpdatePlaylistDto editedPlaylist);
        Task<GenericResultDto<PlaylistDto>> AddBooksToPlaylist(int playlistId, List<int>booksIds, string userId);
        Task<GenericResultDto<PlaylistDto>> RemoveBooksFromPlaylist(int playlistId, List<int> booksIds, string userId);
        Task<GenericResultDto<List<PlaylistDto>>> GetAllPlaylistsAsync(int page);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddDiscussionAsync(AddDiscussionArticleNewsDto discussion);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> GetDiscussionAsync(int discussionId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> UpdateDiscussionAsync(UpdateDiscussionArticleNewsDto editedDiscussion);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddBooksToDiscussion(int discussionId, List<int> booksIds, string userId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> RemoveBooksFromDiscussion(int discussionId, List<int> booksIds, string userId);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag);
        Task LikeAsync(int postId);
        Task<GenericResultDto<string>> Delete(int postId);
        Task<GenericResultDto<List<ReviewDto>>> GetUserReviews(string userId, int page);
        Task<GenericResultDto<List<PlaylistDto>>> GetUserPlaylists(string userId, int page);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetUserDiscussions(string userId, int page);
        Task<GenericResultDto<CommentDto>> AddCommentAsync(CommentDto dto);
        Task<GenericResultDto<string>> DeleteCommentAsync(int commentId);
        Task<GenericResultDto<CommentDto>> GetCommentAsync(int commentId);
        Task<GenericResultDto<List<CommentDto>>> GetPostCommentsAsync(int postId, int page);
        Task<GenericResultDto<CommentDto>> LikeCommentsAsync(int commentId);
        Task<GenericResultDto<CommentDto>> UpdateCommentAsync(int commentId, string content);
        Task<GenericResultDto<ArrayList>> GetFollowingsPostsAsync(IEnumerable<string> followingsIds, int page);
        Task<GenericResultDto<ArrayList>> GetPostsAsync(int page);
    }
}
