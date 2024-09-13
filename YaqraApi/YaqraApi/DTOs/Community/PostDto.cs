using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.User;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

namespace YaqraApi.DTOs.Community
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; } = false;   
        public DateTime CreatedDate { get; set; }
    }
    public class ReviewDto : PostDto
    {

        [Precision(4, 2), Range(1, 10)]
        public decimal Rate { get; set; }
        public BookDto Book { get; set; }
        public string Type { get; set; } = "Review";

    }
    public class PlaylistDto : PostDto
    {
        public List<BookDto> Books { get; set; }
        public string Type { get; set; } = "Playlist";

    }
    public class DiscussionArticlesNewsDto : PostDto
    {
        public string Tag { get; set; }
        public List<BookDto>? Books { get; set; }
        public string Type { get; set; } = "DiscussionArticleNews";

    }
}
