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
        Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(UpdateReviewDto editedReview, string userId);
        Task<GenericResultDto<List<ReviewDto>>> GetAllReviewsAsync(int page);
        Task<GenericResultDto<PlaylistDto>> AddPlaylistAsync(AddPlaylistDto playlist);
        Task<GenericResultDto<PlaylistDto>> GetPlaylistAsync(int playlistId);
        Task<GenericResultDto<PlaylistDto>> UpdatePlaylistAsync(UpdatePlaylistDto editedPlaylist, string userId);
        Task<GenericResultDto<PlaylistDto>> AddBooksToPlaylist(int playlistId, HashSet<int>booksIds, string userId);
        Task<GenericResultDto<PlaylistDto>> RemoveBooksFromPlaylist(int playlistId, HashSet<int> booksIds, string userId);
        Task<GenericResultDto<List<PlaylistDto>>> GetAllPlaylistsAsync(int page);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddDiscussionAsync(AddDiscussionArticleNewsDto discussion);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> GetDiscussionAsync(int discussionId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> UpdateDiscussionAsync(UpdateDiscussionArticleNewsDto editedDiscussion, string userId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> AddBooksToDiscussion(int discussionId, HashSet<int> booksIds, string userId);
        Task<GenericResultDto<DiscussionArticlesNewsDto>> RemoveBooksFromDiscussion(int discussionId, HashSet<int> booksIds, string userId);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag);
        Task<GenericResultDto<LikeDto>> LikeAsync(int postId, string userId);
        Task<GenericResultDto<string>> Delete(int postId, string userId);
        Task<GenericResultDto<List<ReviewDto>>> GetUserReviews(string userId, int page);
        Task<GenericResultDto<List<PlaylistDto>>> GetUserPlaylists(string userId, int page);
        Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetUserDiscussions(string userId, int page);
        Task<GenericResultDto<CommentDto>> AddCommentAsync(CommentDto dto);
        Task<GenericResultDto<string>> DeleteCommentAsync(int commentId, string userId);
        Task<GenericResultDto<CommentDto>> GetCommentAsync(int commentId);
        Task<GenericResultDto<List<CommentDto>>> GetPostCommentsAsync(int postId, int page);
        Task<GenericResultDto<CommentDto>> LikeCommentsAsync(int commentId, string userId);
        Task<GenericResultDto<CommentDto>> UpdateCommentAsync(int commentId, string content, string userId);
        Task<GenericResultDto<ArrayList>> GetFollowingsPostsAsync(IEnumerable<string> followingsIds, int page);
        Task<GenericResultDto<ArrayList>> GetPostsAsync(int page);
        Task<GenericResultDto<string>> GetPostUserIdAsync(int postId);
        Task<GenericResultDto<string>> GetCommentUserIdAsync(int commentId);
        Task<GenericResultDto<Post>> GetPostAsync(int postId);
        Task<bool> IsPostLikedAsync(int postId, string userId);
        Task<HashSet<int>> ArePostsLiked(List<int> postsIds, string userId);
    }
}
