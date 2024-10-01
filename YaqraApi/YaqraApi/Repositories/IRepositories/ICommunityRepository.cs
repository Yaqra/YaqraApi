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
        int GetAllReviewsCount();
        Task<List<Review>> GetAllReviewsAsync(int page);
        int GetAllPlaylistsCount();
        Task<List<Playlist>> GetAllPlaylistsAsync(int page);
        int GetAllDiscussionsCount(DiscussionArticleNewsTag tag);
        Task<List<DiscussionArticleNews>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag);
        void UpdatePost(Post post);
        void Delete(Post post);
        int GetUserReviewsCount(string userId);
        Task<List<Review>?> GetUserReviews(string userId, int page);
        int GetUserPlaylistsCount(string userId);
        Task<List<Playlist>?> GetUserPlaylists(string userId, int page);
        int GetUserDiscussionsCount(string userId);
        Task<List<DiscussionArticleNews>?> GetUserDiscussions(string userId, int page);
        Task<Comment?> AddCommentAsync(Comment comment);
        void DeleteComment(Comment comment);
        Task<Comment?> GetCommentAsync(int commentId);

        Task<int> GetPostCommentsCount(int postId);
        Task<List<Comment>> GetPostCommentsAsync(int postId, int page);
        Comment UpdateComment(Comment comment);
        Task<List<Post>> GetFollowingsPostsAsync(IEnumerable<string> followersIds, int page);
        Task<List<Post>> GetPostsAsync(int page);
        Task<string?> GetPostUserIdAsync(int postId);
        Task<string?> GetCommentUserIdAsync(int commentId);
        Task<bool> IsPostLiked(int postId, string userId);
        Task<HashSet<int>> ArePostsLiked(HashSet<int> postsIds, string userId);
        Task<bool> IsCommentLiked(int commentId, string userId);
        Task<HashSet<int>> AreCommentsLiked(HashSet<int> commentsIds, string userId);
        Task LoadPlaylistBooks(Playlist playlist);

    }
}
