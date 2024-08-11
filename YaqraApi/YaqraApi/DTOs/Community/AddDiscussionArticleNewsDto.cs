using System.Text.Json.Serialization;
using YaqraApi.Models.Enums;

namespace YaqraApi.DTOs.Community
{
    public class AddDiscussionArticleNewsDto
    {
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public List<int>? BooksIds { get; set; }
        public DiscussionArticleNewsTag Tag { get; set; } = DiscussionArticleNewsTag.Discussion;
    }
}
