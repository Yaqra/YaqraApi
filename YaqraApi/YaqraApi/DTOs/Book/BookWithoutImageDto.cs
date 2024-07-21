namespace YaqraApi.DTOs.Book
{
    public class BookWithoutImageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
