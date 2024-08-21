namespace YaqraApi.DTOs.Book
{
    public class BookFinderDto
    {
        public decimal? MinimumRate { get; set; }
        public HashSet<int>? AuthorIds { get; set; }
        public HashSet<int>? GenreIds { get; set; }
        public int Page { get; set; }
    }
}
