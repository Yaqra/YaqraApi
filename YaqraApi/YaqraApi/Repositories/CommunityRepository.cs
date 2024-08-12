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
    }
}
