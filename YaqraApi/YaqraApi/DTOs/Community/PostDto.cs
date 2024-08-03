using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YaqraApi.Models;

namespace YaqraApi.DTOs.Community
{
    public class PostDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ReviewDto : PostDto
    {

        [Precision(4, 2), Range(1, 10)]
        public decimal Rate { get; set; }
        public int BookId { get; set; }
    }
}
