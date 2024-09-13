using Microsoft.EntityFrameworkCore;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
namespace YaqraApi.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly ApplicationContext _context;

        public CommunityRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<Review?> AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await SaveChangesAsync();

            _context.Entry(review).State = EntityState.Detached;

            return review.Id == 0 ? null : review;
        }

        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<Review?> GetReviewAsync(int reviewId)
        {
            return await (_context.Reviews
                .Include(r=>r.Book)
                    .ThenInclude(b=>b.Genres)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Authors)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Id == reviewId));
        }

        public void Delete(Post post)
        {
            _context.Posts.Remove(post);
            SaveChanges();
        }

        public async Task<Review?> UpdateReviewAsync(Review editedReview)
        {
            _context.Reviews.Update(editedReview);
            SaveChanges();
            return editedReview;
        }

        public async Task<Playlist?> AddPlaylistAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await SaveChangesAsync();

            _context.Entry(playlist).State = EntityState.Detached;

            return playlist.Id == 0 ? null : playlist;
        }

        public async Task<Playlist?> GetPlaylistAsync(int playlistId)
        {
            return await(_context.Playlists
                .Include(p=>p.Books)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Id == playlistId));
        }

        public async Task<Playlist?> UpdatePlaylistAsync(Playlist editedPlaylist)
        {
            _context.Playlists.Update(editedPlaylist);
            SaveChanges();
            return editedPlaylist;
        }

        public async Task<Post?> GetPostAsync(int postId)
        {
            return await(_context.Posts
                .Include(r => r.User)
                .Include(r=>r.PostLikes)
                .SingleOrDefaultAsync(r => r.Id == postId));
        }

        public async Task<DiscussionArticleNews?> AddDiscussionAsync(DiscussionArticleNews discussion)
        {
            await _context.DiscussionArticleNews.AddAsync(discussion);
            await SaveChangesAsync();

            _context.Entry(discussion).State = EntityState.Detached;

            return discussion.Id == 0 ? null : discussion;
        }

        public async Task<DiscussionArticleNews?> GetDiscussionAsync(int discussionId)
        {
            return await(_context.DiscussionArticleNews
                .Include(p => p.Books)
                .Include(r => r.User)
                .SingleOrDefaultAsync(d => d.Id == discussionId));
        }

        public async Task<DiscussionArticleNews?> UpdateDiscussionAsync(DiscussionArticleNews editedDiscussion)
        {
            _context.DiscussionArticleNews.Update(editedDiscussion);
            SaveChanges();
            return editedDiscussion;
        }

        public async Task<List<Review>> GetAllReviewsAsync(int page)
        {
            return await (_context.Reviews
                .Include(r => r.Book)
                    .ThenInclude(b => b.Genres)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Authors)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .ToListAsync();
        }

        public async Task<List<Playlist>> GetAllPlaylistsAsync(int page)
        {
            return await(_context.Playlists
                .Include(r => r.Books)
                    .ThenInclude(b => b.Genres)
                .Include(r => r.Books)
                    .ThenInclude(b => b.Authors)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .ToListAsync();
        }

        public async Task<List<DiscussionArticleNews>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag)
        {
            return await(_context.DiscussionArticleNews
                .Where(d=>d.Tag == tag)
                .Include(r => r.Books)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .ToListAsync();
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            SaveChanges();
        }

        public async Task<List<Review>?> GetUserReviews(string userId, int page)
        {
            return await (_context.Reviews
                .Where(r=>r.UserId == userId)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Genres)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Authors)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Playlist>?> GetUserPlaylists(string userId, int page)
        {
            return await(_context.Playlists
                .Where(r => r.UserId == userId)
                .Include(r => r.Books)
                    .ThenInclude(b => b.Genres)
                .Include(r => r.Books)
                    .ThenInclude(b => b.Authors)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<DiscussionArticleNews>?> GetUserDiscussions(string userId, int page)
        {
            return await (_context.DiscussionArticleNews
                .Where(r => r.UserId == userId && r.Tag == DiscussionArticleNewsTag.Discussion)
                .Include(r => r.Books)
                .Include(r => r.User))
                .Skip((page - 1) * Pagination.Posts).Take(Pagination.Posts)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Comment?> AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await SaveChangesAsync();

            _context.Entry(comment).State = EntityState.Detached;

            return comment.Id == 0 ? null : comment;
        }

        public void DeleteComment(Comment comment)
        {
            foreach (var reply in comment.Replies)
                DeleteComment(reply);

            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }

        private void LoadReplies(Comment comment)
        {
            _context.Entry(comment).Collection(c => c.Replies).Load();

            foreach (var reply in comment.Replies)
                LoadReplies(reply);
        }

        public async Task<Comment?> GetCommentAsync(int commentId)
        {
            var comment = await _context.Comments
                .Include(c=>c.User)
                .Include(c=>c.CommentLikes)
                .SingleOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                return null;
            LoadReplies(comment);
            return comment;
        }

        public async Task<List<Comment>> GetPostCommentsAsync(int postId, int page)
        {
           var post = (await _context.Posts
                .Include(p=>
                    p.Comments
                    .Skip((page - 1) * Pagination.Comments).Take(Pagination.Comments)
                    .OrderBy(c=>c.CreatedDate)
                )
                .ThenInclude(c=>c.User)
                .SingleOrDefaultAsync(p=>p.Id == postId));
            if (post == null)
                return null;
            var comments = post.Comments;
            foreach (var comment in comments)
            {
                LoadReplies(comment);
            }
            return comments.ToList();
        }
        public Comment UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            SaveChanges();
            return comment;
        }

        public async Task<List<Post>> GetFollowingsPostsAsync(IEnumerable<string> followingsIds, int page)
        {
            var posts = _context.Posts
            .Where(r => followingsIds.Contains(r.UserId))
            .Include(r => (r as Playlist).Books)
            .Include(r => (r as DiscussionArticleNews).Books)
            .Include(r => (r as Review).Book)
            .Include(r => r.User)
            .Skip((page - 1) * Pagination.Timeline).Take(Pagination.Timeline)
            .OrderByDescending(r => r.Id);
            return await posts.ToListAsync();
        }

        public async Task<List<Post>> GetPostsAsync(int page)
        {
            var posts = _context.Posts
            .Include(r => (r as Playlist).Books)
            .Include(r => (r as DiscussionArticleNews).Books)
            .Include(r => (r as Review).Book)
            .Include(r => r.User)
            .Skip((page - 1) * Pagination.Timeline).Take(Pagination.Timeline)
            .OrderByDescending(r => r.Id);
            return await posts.ToListAsync();
        }

        public async Task<string?> GetPostUserIdAsync(int postId)
        {
            return (await _context.Posts.SingleOrDefaultAsync(p => p.Id == postId))?.UserId;
        }

        public async Task<string?> GetCommentUserIdAsync(int commentId)
        {
            return (await _context.Comments.SingleOrDefaultAsync(p => p.Id == commentId))?.UserId;
        }

        public async Task<bool> IsPostLiked(int postId, string userId)
        {
            return (await _context.PostLikes.AnyAsync(x => x.PostId == postId && x.UserId == userId));
        }

        public async Task<HashSet<int>> ArePostsLiked(HashSet<int> postsIds, string userId)
        {
            var result  = _context.PostLikes.Where(x => x.UserId == userId && postsIds.Contains(x.PostId));
            return new HashSet<int>(await result.Select(r=>r.PostId).ToListAsync());
        }

        public async Task<bool> IsCommentLiked(int commentId, string userId)
        {
            return (await _context.CommentLikes.AnyAsync(x => x.CommentId == commentId && x.UserId == userId));
        }

        public async Task<HashSet<int>> AreCommentsLiked(HashSet<int> commentsIds, string userId)
        {
            var result = _context.CommentLikes.Where(x => x.UserId == userId && commentsIds.Contains(x.CommentId));
            return new HashSet<int>(await result.Select(r => r.CommentId).ToListAsync());
        }
    }
}
