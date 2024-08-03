using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YaqraApi.DTOs.Community
{
    public class AddReviewDto
    {
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Precision(4, 2), Range(1, 10)]
        public decimal Rate { get; set; }
        public int BookId { get; set; }
    }
}
