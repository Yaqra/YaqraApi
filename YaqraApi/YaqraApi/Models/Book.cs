namespace YaqraApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        //public decimal Rate { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public ICollection<UserBookWithStatus> UserBooks { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Author> Authors { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<DiscussionArticleNews> DiscussionArticleNews { get; set; }


    }
}
