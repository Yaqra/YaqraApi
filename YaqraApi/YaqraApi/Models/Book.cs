namespace YaqraApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        //public double Rate { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public ICollection<UserBookWithStatus> UserBooks { get; set; }

    }
}
