using System.ComponentModel.DataAnnotations.Schema;

namespace YaqraApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public Post Post { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int LikeCount { get; set; }
        public string Content { get; set; }
        [ForeignKey(nameof(ParentComment))]
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment>? Replies { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
