using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace YaqraApi.DTOs.Genre
{
    public class GenreDto
    {
        [Required]
        public int GenreId { get; set; }
        [Required]
        public string GenreName { get; set; }
    }
}
