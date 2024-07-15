using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.Genre
{
    public class GenreIdDto
    {
        [Required]
        public int GenreId { get; set; }
    }
}
