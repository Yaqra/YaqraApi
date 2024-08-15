using System.ComponentModel.DataAnnotations.Schema;

namespace YaqraApi.Models
{
    public class TrendingBook
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime AddedDate{ get; set; } = DateTime.UtcNow;
    }
}
