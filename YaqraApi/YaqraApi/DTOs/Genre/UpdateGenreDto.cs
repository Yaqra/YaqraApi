using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.Genre
{
    public class UpdateGenreDto
    {
        [Required]
        public int CurrentGenreId { get; set; }
        [Required]
        public string NewGenreName { get; set; }
    }
}
