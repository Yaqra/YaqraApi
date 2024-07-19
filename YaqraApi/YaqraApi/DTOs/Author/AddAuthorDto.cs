using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.Author
{
    public class AddAuthorDto
    {
        [Required]
        public string Name { get; set; }
        public string? Bio { get; set; }
    }
}
