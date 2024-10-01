using System.ComponentModel.DataAnnotations;
using YaqraApi.Models.Enums;

namespace YaqraApi.DTOs.Community
{
    public class UpdatePostDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
    }
    public class UpdateReviewDto: UpdatePostDto
    {
        [Range(1, 10)]
        public decimal Rate { get; set; }
        public int BookId { get; set; }
    }
    public class UpdatePlaylistDto : UpdatePostDto
    {
        public List<int> BooksIds { get; set; }
    }
    public class UpdateDiscussionArticleNewsDto : UpdatePostDto
    {
        public List<int>? BooksIds { get; set; }
        public DiscussionArticleNewsTag Tag { get; set; }
    }
}
