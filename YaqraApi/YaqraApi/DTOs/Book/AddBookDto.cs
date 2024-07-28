using Microsoft.AspNetCore.Mvc;

namespace YaqraApi.DTOs.Book
{
    public class AddBookDto
    {
        public IFormFile? Image { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        public List<int>? GenresIds { get; set; }
        public List<int> AuthorsIds { get; set; }
    }
}
