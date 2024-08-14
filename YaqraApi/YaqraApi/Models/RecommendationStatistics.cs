using System.ComponentModel.DataAnnotations.Schema;

namespace YaqraApi.Models
{
    public class RecommendationStatistics
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int Points { get; set; } = 1;
    }
}
