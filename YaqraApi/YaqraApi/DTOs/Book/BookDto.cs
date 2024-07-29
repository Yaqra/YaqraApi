using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Genre;
using YaqraApi.Models;

namespace YaqraApi.DTOs.Book
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public List<AuthorDto> AuthorsDto { get; set; } = new List<AuthorDto>();
        public List<GenreDto> GenresDto { get; set; } = new List<GenreDto>();
    }
}
