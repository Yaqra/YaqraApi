using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using YaqraApi.Models.Enums;

namespace YaqraApi.Models
{
    public class Post
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
    public class Review : Post
    {
        [Precision(4, 2)]
        public decimal Rate { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book{ get; set; }
    }
    public class Playlist : Post
    {
        public ICollection<Book> Books { get; set; }
    }
    public class DiscussionArticleNews : Post
    {
        public DiscussionArticleNewsTag Tag { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
