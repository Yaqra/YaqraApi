using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class BioDto
    {
        [Required]
        public string NewBio { get; set; }
    }
}
