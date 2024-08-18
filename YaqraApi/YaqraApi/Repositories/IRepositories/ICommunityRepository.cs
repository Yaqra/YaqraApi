using System.Collections;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

namespace YaqraApi.Repositories.IRepositories
{
    public interface ICommunityRepository
    {
        Task<Review?> AddReviewAsync(Review review);
        Task<Review?> GetReviewAsync(int reviewId);
        Task<Post?> GetPostAsync(int postId);
        Task<Review?> UpdateReviewAsync(Review editedReview);
        Task<Playlist?> AddPlaylistAsync(Playlist playlist);
        Task<Playlist?> GetPlaylistAsync(int playlistId);
        Task<Playlist?> UpdatePlaylistAsync(Playlist editedPlaylist);
        Task<DiscussionArticleNews?> AddDiscussionAsync(DiscussionArticleNews discussion);
        Task<DiscussionArticleNews?> GetDiscussionAsync(int discussionId);
        Task<DiscussionArticleNews?> UpdateDiscussionAsync(DiscussionArticleNews editedDiscussion);
        Task<List<Review>> GetAllReviewsAsync(int page);        
        Task<List<Playlist>> GetAllPlaylistsAsync(int page);
        Task<List<DiscussionArticleNews>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag);
        void UpdatePost(Post post);
        void Delete(Post post);
        Task<List<Review>?> GetUserReviews(string userId, int page);
        Task<List<Playlist>?> GetUserPlaylists(string userId, int page);
        Task<List<DiscussionArticleNews>?> GetUserDiscussions(string userId, int page);
        Task<Comment?> AddCommentAsync(Comment comment);
        void DeleteComment(Comment comment);
        Task<Comment?> GetCommentAsync(int commentId);
        Task<List<Comment>> GetPostCommentsAsync(int postId, int page);
        Comment UpdateComment(Comment comment);
        Task<List<Post>> GetFollowingsPostsAsync(IEnumerable<string> followersIds, int page);
        Task<List<Post>> GetPostsAsync(int page);
        Task<string?> GetPostUserIdAsync(int postId);
        Task<string?> GetCommentUserIdAsync(int commentId);

    }
}
