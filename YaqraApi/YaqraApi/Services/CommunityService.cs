using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Net;
using System.Security.Cryptography.Xml;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Community;
using YaqraApi.DTOs.Genre;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IBookService _bookService;
        private readonly IRecommendationService _recommendationService;
        private readonly Mapper _mapper;
        public CommunityService(ICommunityRepository communityRepository, 
            IBookService bookService,
            IRecommendationService recommendationService)
        {
            _communityRepository = communityRepository;
            _bookService = bookService;
            _recommendationService = recommendationService;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        private Playlist AddBooksToPlaylist(Playlist playlist, List<int> BooksIds)
        {
            foreach (var id in BooksIds)
                playlist.Books.Add(new Book { Id = id });
            _bookService.Attach(playlist.Books);
            return playlist;
        }
        private DiscussionArticleNews AddBooksToDiscussion(DiscussionArticleNews discussion, List<int> BooksIds)
        {
            if(discussion.Books == null)
                discussion.Books = new List<Book>();

            foreach (var id in BooksIds)
                discussion.Books.Add(new Book { Id = id });
            _bookService.Attach(discussion.Books);
            return discussion;
        }
        public async Task<GenericResultDto<PlaylistDto>> AddBooksToPlaylist(int playlistId, List<int> booksIds, string userId)
        {
            var playlist = await _communityRepository.GetPlaylistAsync(playlistId);
            if (playlist == null)
                return new GenericResultDto<PlaylistDto> { Succeeded = false, ErrorMessage = "playlist not found" };

            playlist = AddBooksToPlaylist(playlist, booksIds);

            foreach (var bookId in booksIds)
            {
                var bookResult = await _bookService.GetByIdAsync(bookId);
                if(bookResult.Succeeded == true)
                {
                    var book = bookResult.Result;
                    await _bookService.AddTrendingBook(book.Id);

                    foreach (var genreId in book.GenresDto.Select(g=>g.GenreId))
                    {
                        await _recommendationService.IncrementPoints(userId, genreId);
                    }
                }
            }

            await _communityRepository.UpdatePlaylistAsync(playlist);
            return new GenericResultDto<PlaylistDto> { Succeeded = true, Result = (await GetPlaylistAsync(playlistId)).Result };
        }

        public async Task<GenericResultDto<PlaylistDto>> AddPlaylistAsync(AddPlaylistDto playlistDto)
        {
            var playlist = _mapper.Map<Playlist>(playlistDto);
            var original = new List<Book>();
            foreach (var id in playlistDto.BooksIds)
                original.Add(new Book { Id = id });
            _bookService.Attach(original);

            playlist.Books = original;
            playlist = await _communityRepository.AddPlaylistAsync(playlist);
            if(playlist == null)
                return new GenericResultDto<PlaylistDto> {Succeeded = false, ErrorMessage = "something went wrong while posting ur playlist" };

            var result = await GetPlaylistAsync(playlist.Id);
            if(result.Succeeded==false)
                return new GenericResultDto<PlaylistDto> { Succeeded = true, ErrorMessage = "ur playlist had been posted successfully but something went wrong while retreiving it" };

            return new GenericResultDto<PlaylistDto> { Succeeded = true, Result = result.Result};
        }

        public async Task<GenericResultDto<ReviewDto>> AddReviewAsync(AddReviewDto review, string userId)
        {
            var original = _mapper.Map<Review>(review);
            
            original.Book = null;


            var bookResult = await _bookService.GetByIdAsync(review.BookId);
            if (bookResult.Succeeded == true)
            {
                var book = bookResult.Result;
                await _bookService.AddTrendingBook(book.Id);
                foreach (var genreId in book.GenresDto.Select(g => g.GenreId))
                {
                    await _recommendationService.IncrementPoints(userId, genreId);
                }
            }
            

            var result = await _communityRepository.AddReviewAsync(original);
            if (result == null)
                return new GenericResultDto<ReviewDto> { Succeeded = false, ErrorMessage = "something went wrong" };
            var resultReview = await GetReviewAsync(result.Id);
            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = resultReview.Result };
        }

        public async Task<GenericResultDto<string>> Delete(int postId)
        {
            var post = await _communityRepository.GetPostAsync(postId);
            if (post == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "no post with that id" };
            _communityRepository.Delete(post);

            return new GenericResultDto<string> { Succeeded = true, Result = "post deleted successfully" }; 
        }

        public async Task<GenericResultDto<PlaylistDto>> GetPlaylistAsync(int playlistId)
        {
            var playlist = await _communityRepository.GetPlaylistAsync(playlistId);
            if (playlist == null)
                return new GenericResultDto<PlaylistDto> { Succeeded = false, ErrorMessage = "no playlist with that id was found" };

            var booksDto = new List<BookDto>();
            foreach (var book in playlist.Books)
                booksDto.Add((await _bookService.GetByIdAsync(book.Id)).Result);

            var result = _mapper.Map<PlaylistDto>(playlist);
            result.Books = booksDto;
            return new GenericResultDto<PlaylistDto> { Succeeded = true, Result = result};
        }

        public async Task<GenericResultDto<ReviewDto>> GetReviewAsync(int reviewId)
        {
            var review = await _communityRepository.GetReviewAsync(reviewId);
            if (review == null)
                return new GenericResultDto<ReviewDto> { Succeeded = false, ErrorMessage = "no review with that id was found" };

            var dto = _mapper.Map<ReviewDto>(review);
            dto.Book = _mapper.Map<BookDto>(review.Book);
            foreach (var genre in review.Book.Genres)
                dto.Book.GenresDto.Add(new GenreDto { GenreId = genre.Id, GenreName = genre.Name});
            foreach (var author in review.Book.Authors)
                dto.Book.AuthorsDto.Add(_mapper.Map<AuthorDto>(author));
            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = dto};
        }

        public async Task<GenericResultDto<PlaylistDto>> UpdatePlaylistAsync(UpdatePlaylistDto editedPlaylist)
        {
            var playlist = _mapper.Map<Playlist>(editedPlaylist);
            var result = await _communityRepository.UpdatePlaylistAsync(playlist);
            if (result == null)
                return new GenericResultDto<PlaylistDto> { Succeeded = false, ErrorMessage = "something went wrong" };

            var dto = (await GetPlaylistAsync(result.Id)).Result;
            return new GenericResultDto<PlaylistDto> { Succeeded = true, Result = dto };
        }

        public async Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(UpdateReviewDto editedReview)
        {
            var review = _mapper.Map<Review>(editedReview);
            var result = await _communityRepository.UpdateReviewAsync(review);
            if (result == null)
                return new GenericResultDto<ReviewDto> { Succeeded = false, ErrorMessage = "something went wrong" };
            var dto = (await GetReviewAsync(result.Id)).Result;
            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = dto };
        }

        public async Task<GenericResultDto<PlaylistDto>> RemoveBooksFromPlaylist(int playlistId, List<int> booksIds, string userId)
        {
            var playlist = await _communityRepository.GetPlaylistAsync(playlistId);
            if (playlist == null)
                return new GenericResultDto<PlaylistDto> { Succeeded = false, ErrorMessage = "playlist not found" };
            
            var booksToRemove = playlist.Books.Where(g => booksIds.Contains(g.Id));

            foreach (var bookId in booksIds)
            {
                var bookResult = await _bookService.GetByIdAsync(bookId);
                if (bookResult.Succeeded == true)
                {
                    var book = bookResult.Result;
                    foreach (var genreId in book.GenresDto.Select(g => g.GenreId))
                    {
                        await _recommendationService.DecrementPoints(userId, genreId);
                    }
                }
            }

            _bookService.Attach(booksToRemove);

            foreach (var book in booksToRemove)
                playlist.Books.Remove(book);

            await _communityRepository.UpdatePlaylistAsync(playlist);
            var result = await GetPlaylistAsync(playlistId);
            if (result.Succeeded == false)
                return new GenericResultDto<PlaylistDto> { Succeeded = true, ErrorMessage = "playlist updated successfully but something went wrong while retrieving it" };
            return result;
        }
        public async Task<GenericResultDto<DiscussionArticlesNewsDto>> GetDiscussionAsync(int discussionId)
        {
            var discussion = await _communityRepository.GetDiscussionAsync(discussionId);
            if (discussion == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "no discussion with that id was found" };

            var booksDto = new List<BookDto>();
            if(discussion.Books != null)
            {
                foreach (var book in discussion.Books)
                    booksDto.Add((await _bookService.GetByIdAsync(book.Id)).Result);
            }

            var result = _mapper.Map<DiscussionArticlesNewsDto>(discussion);
            result.Books = booksDto;
            return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<DiscussionArticlesNewsDto>> AddDiscussionAsync(AddDiscussionArticleNewsDto discussionDto)
        {
            var discussion = _mapper.Map<DiscussionArticleNews>(discussionDto);
            var original = new List<Book>();
            if(discussionDto.BooksIds != null)
            {
                foreach (var id in discussionDto.BooksIds)
                    original.Add(new Book { Id = id });
                _bookService.Attach(original);
            }
            discussion.Books = original;
            discussion = await _communityRepository.AddDiscussionAsync(discussion);
            if (discussion == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "something went wrong while posting ur discussion" };

            var result = await GetDiscussionAsync(discussion.Id);
            if (result.Succeeded == false)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, ErrorMessage = "ur discussion had been posted successfully but something went wrong while retreiving it" };

            return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, Result = result.Result };
        }

        public async Task<GenericResultDto<DiscussionArticlesNewsDto>> UpdateDiscussionAsync(UpdateDiscussionArticleNewsDto editedDiscussion)
        {
            var discussion = _mapper.Map<DiscussionArticleNews>(editedDiscussion);
            var result = await _communityRepository.UpdateDiscussionAsync(discussion);
            if (result == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "something went wrong" };
            var dto = (await GetDiscussionAsync(result.Id)).Result;
            return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, Result = dto };
        }

        public async Task<GenericResultDto<DiscussionArticlesNewsDto>> AddBooksToDiscussion(int discussionId, List<int> booksIds, string userId)
        {
            var discussion = await _communityRepository.GetDiscussionAsync(discussionId);
            if (discussion == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "discussion not found" };

            foreach (var bookId in booksIds)
            {
                var bookResult = await _bookService.GetByIdAsync(bookId);
                if (bookResult.Succeeded == true)
                {
                    var book = bookResult.Result;
                    await _bookService.AddTrendingBook(book.Id);
                    foreach (var genreId in book.GenresDto.Select(g => g.GenreId))
                    {
                        await _recommendationService.IncrementPoints(userId, genreId);
                    }
                }
            }

            discussion = AddBooksToDiscussion(discussion, booksIds);
            await _communityRepository.UpdateDiscussionAsync(discussion);
            return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, Result = (await GetDiscussionAsync(discussionId)).Result };
        }

        public async Task<GenericResultDto<DiscussionArticlesNewsDto>> RemoveBooksFromDiscussion(int discussionId, List<int> booksIds, string userId)
        {
            var discussion = await _communityRepository.GetDiscussionAsync(discussionId);
            if (discussion == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "discussion not found" };
            if(discussion.Books == null)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = false, ErrorMessage = "books not found" };

            var booksToRemove = discussion.Books.Where(g => booksIds.Contains(g.Id));

            foreach (var bookId in booksIds)
            {
                var bookResult = await _bookService.GetByIdAsync(bookId);
                if (bookResult.Succeeded == true)
                {
                    var book = bookResult.Result;
                    foreach (var genreId in book.GenresDto.Select(g => g.GenreId))
                    {
                        await _recommendationService.DecrementPoints(userId, genreId);
                    }
                }
            }

            _bookService.Attach(booksToRemove);

            foreach (var book in booksToRemove)
                discussion.Books.Remove(book);

            await _communityRepository.UpdateDiscussionAsync(discussion);
            var result = await GetDiscussionAsync(discussionId);
            if (result.Succeeded == false)
                return new GenericResultDto<DiscussionArticlesNewsDto> { Succeeded = true, ErrorMessage = "discussion updated successfully but something went wrong while retrieving it" };
            return result;
        }

        public async Task<GenericResultDto<List<ReviewDto>>> GetAllReviewsAsync(int page)
        {
            page = page == 0 ? 1 : page;
            var reviews = await _communityRepository.GetAllReviewsAsync(page);
            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
                reviewsDto.Add(_mapper.Map<ReviewDto>(review));
            return new GenericResultDto<List<ReviewDto>> { Succeeded = true, Result = reviewsDto };
        }

        public async Task<GenericResultDto<List<PlaylistDto>>> GetAllPlaylistsAsync(int page)
        {
            page = page == 0 ? 1 : page;
            var playlists = await _communityRepository.GetAllPlaylistsAsync(page);
            var playlistsDto = new List<PlaylistDto>();
            foreach (var playlist in playlists)
                playlistsDto.Add(_mapper.Map<PlaylistDto>(playlist));
            return new GenericResultDto<List<PlaylistDto>> { Succeeded = true, Result = playlistsDto };

        }

        public async Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetAllDiscussionsAsync(int page, DiscussionArticleNewsTag tag)
        {
            page = page == 0 ? 1 : page;
            var discussions = await _communityRepository.GetAllDiscussionsAsync(page, tag);
            var discussionsDto = new List<DiscussionArticlesNewsDto>();
            foreach (var discussion in discussions)
                discussionsDto.Add(_mapper.Map<DiscussionArticlesNewsDto>(discussion));
            return new GenericResultDto<List<DiscussionArticlesNewsDto>> { Succeeded = true, Result = discussionsDto };

        }

        public async Task<Post?> LikeAsync(int postId, string userId)
        {
            var post = await _communityRepository.GetPostAsync(postId);
            if (post == null)
                return null;

            var postLike = post.PostLikes.SingleOrDefault(p => p.UserId == userId);
            if (postLike != null)
            {
                post.PostLikes.Remove(postLike);
                post.LikeCount--;
            }
            else
            {
                post.PostLikes.Add(new PostLikes { PostId = postId, UserId = userId });
                post.LikeCount++;
            }
            _communityRepository.UpdatePost(post);
            return post;
        }

        public async Task<GenericResultDto<List<ReviewDto>>> GetUserReviews(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var reviews = await _communityRepository.GetUserReviews(userId, page);
            if (reviews == null)
                return new GenericResultDto<List<ReviewDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
                reviewsDto.Add(_mapper.Map<ReviewDto>(review));

            return new GenericResultDto<List<ReviewDto>> { Succeeded = true, Result = reviewsDto };

        }

        public async Task<GenericResultDto<List<PlaylistDto>>> GetUserPlaylists(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var playlists = await _communityRepository.GetUserPlaylists(userId, page);
            if (playlists == null)
                return new GenericResultDto<List<PlaylistDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var playlistsDto = new List<PlaylistDto>();
            foreach (var playlist in playlists)
                playlistsDto.Add(_mapper.Map<PlaylistDto>(playlist));

            return new GenericResultDto<List<PlaylistDto>> { Succeeded = true, Result = playlistsDto };
        }

        public async Task<GenericResultDto<List<DiscussionArticlesNewsDto>>> GetUserDiscussions(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var discussions = await _communityRepository.GetUserDiscussions(userId, page);
            if (discussions == null)
                return new GenericResultDto<List<DiscussionArticlesNewsDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var discussionsDto = new List<DiscussionArticlesNewsDto>();
            foreach (var dis in discussions)
                discussionsDto.Add(_mapper.Map<DiscussionArticlesNewsDto>(dis));

            return new GenericResultDto<List<DiscussionArticlesNewsDto>> { Succeeded = true, Result = discussionsDto };
        }

        public async Task<GenericResultDto<CommentDto>> AddCommentAsync(CommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);
            comment = await _communityRepository.AddCommentAsync(comment);
            if (comment == null)
                return new GenericResultDto<CommentDto> { Succeeded = false, ErrorMessage = "something went wrong while posting ur comment" };
            return new GenericResultDto<CommentDto> { Succeeded = true, Result = _mapper.Map<CommentDto>(comment) };
        }

        public async Task<GenericResultDto<CommentDto>> GetCommentAsync(int commentId)
        {
            var comment = await _communityRepository.GetCommentAsync(commentId);
            if (comment == null)
                return new GenericResultDto<CommentDto> { Succeeded = false, ErrorMessage = "comment not found" };

            return new GenericResultDto<CommentDto> { Succeeded = true, Result = _mapper.Map<CommentDto>(comment) };
        }

        public async Task<GenericResultDto<string>> DeleteCommentAsync(int commentId)
        {
            var comment = await _communityRepository.GetCommentAsync(commentId);
            if (comment == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "comment not found" };
            _communityRepository.DeleteComment(comment);
            return new GenericResultDto<string> { Succeeded = true, Result = "comment deleted successfully" };
        }

        public async Task<GenericResultDto<List<CommentDto>>> GetPostCommentsAsync(int postId, int page)
        {
            page = page == 0 ? 1 : page;
            var comments = await _communityRepository.GetPostCommentsAsync(postId, page);
            if (comments == null)
                return new GenericResultDto<List<CommentDto>> { Succeeded = false, ErrorMessage = "post not found" };
            List<CommentDto> result = new List<CommentDto>();
            foreach (var comment in comments)
                result.Add(_mapper.Map<CommentDto>(comment));

            return new GenericResultDto<List<CommentDto>> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<CommentDto>> LikeCommentsAsync(int commentId, string userId)
        {
            var comment = await _communityRepository.GetCommentAsync(commentId);

            if (comment == null)
                return new GenericResultDto<CommentDto> { Succeeded = false, ErrorMessage = "comment not found" };


            var commentLike = comment.CommentLikes.SingleOrDefault(p => p.UserId == userId);
            if (commentLike != null)
            {
                comment.CommentLikes.Remove(commentLike);
                comment.LikeCount--;
            }
            else
            {
                comment.CommentLikes.Add(new CommentLikes { CommentId = commentId, UserId = userId });
                comment.LikeCount++;
            }

            _communityRepository.UpdateComment(comment);
            return new GenericResultDto<CommentDto> { Succeeded = true, Result = _mapper.Map<CommentDto>(comment) };

        }

        public async Task<GenericResultDto<CommentDto>> UpdateCommentAsync(int commentId, string content)
        {
            var comment = await _communityRepository.GetCommentAsync(commentId);
            if (comment == null)
                return new GenericResultDto<CommentDto> { Succeeded = false, ErrorMessage = "comment not found" };
            comment.Content = content;
            comment = _communityRepository.UpdateComment(comment);
            return new GenericResultDto<CommentDto> { Succeeded = true, Result = _mapper.Map<CommentDto>(comment) };
        }

        public async Task<GenericResultDto<ArrayList>> GetFollowingsPostsAsync(IEnumerable<string> followingsIds, int page)
        {
            var posts = await _communityRepository.GetFollowingsPostsAsync(followingsIds, page);
            if (posts.IsNullOrEmpty())
                return new GenericResultDto<ArrayList> { Succeeded = true, Result = new ArrayList() };
            return new GenericResultDto<ArrayList> { Succeeded = true, Result = ConvertPostParentToChildren(posts) };
        }
        
        private ArrayList ConvertPostParentToChildren(List<Post> posts)
        {
            var arr = new ArrayList();
            foreach (var post in posts)
            {
                if(post is Review)
                {
                    var review = post as Review;
                    var reviewDto = _mapper.Map<ReviewDto>(review);
                    reviewDto.Book = _mapper.Map<BookDto>(review.Book);
                    arr.Add(reviewDto);
                }
                else if (post is Playlist)
                {
                    var playlistDto = _mapper.Map<PlaylistDto>(post as Playlist);
                    arr.Add(playlistDto);
                }
                else // discussionArticleNews
                {
                    var discussionDto = _mapper.Map<DiscussionArticlesNewsDto>(post as DiscussionArticleNews);
                    arr.Add(discussionDto);
                }
            }
            return arr;
        }

        public async Task<GenericResultDto<ArrayList>> GetPostsAsync(int page)
        {
            var posts = await _communityRepository.GetPostsAsync(page);
            if (posts.IsNullOrEmpty())
                return new GenericResultDto<ArrayList> { Succeeded = true, Result = new ArrayList() };
            return new GenericResultDto<ArrayList> { Succeeded = true, Result = ConvertPostParentToChildren(posts) };
        }

        public async Task<GenericResultDto<string>> GetPostUserIdAsync(int postId)
        {
            var userId = await _communityRepository.GetPostUserIdAsync(postId);
            if (userId == null)
                return new GenericResultDto<string> { Succeeded = false };
            return new GenericResultDto<string> { Succeeded = true, Result = userId };
        }

        public async Task<GenericResultDto<string>> GetCommentUserIdAsync(int commentId)
        {
            var userId = await _communityRepository.GetCommentUserIdAsync(commentId);
            if (userId == null)
                return new GenericResultDto<string> { Succeeded = false };
            return new GenericResultDto<string> { Succeeded = true, Result = userId };
        }
    }
}
