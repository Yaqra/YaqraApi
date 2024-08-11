using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YaqraApi.DTOs.Community
{
    public class AddPlaylistDto
    {
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public List<int> BooksIds { get; set; }
    }
}
