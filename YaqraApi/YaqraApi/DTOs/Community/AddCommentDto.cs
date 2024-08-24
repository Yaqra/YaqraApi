using YaqraApi.DTOs.User;

namespace YaqraApi.DTOs.Community
{
    public class AddCommentDto
    {
        public int PostId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string Content { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
