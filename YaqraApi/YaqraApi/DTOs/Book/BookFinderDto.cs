namespace YaqraApi.DTOs.Book
{
    public class BookFinderDto
    {
        public decimal? MinimumRate { get; set; }
        public List<int>? AuthorIds { get; set; }
        public List<int>? GenreIds { get; set; }
        public int Page { get; set; }
    }
}
