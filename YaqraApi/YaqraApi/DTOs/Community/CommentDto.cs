using System.ComponentModel.DataAnnotations.Schema;
using YaqraApi.Models;

namespace YaqraApi.DTOs.Community
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int LikeCount { get; set; }
        public string Content { get; set; }
        public ICollection<CommentDto>? Replies { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
