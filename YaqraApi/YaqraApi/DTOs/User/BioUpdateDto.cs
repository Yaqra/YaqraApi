using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class BioUpdateDto
    {
        [Required]
        public string NewBio { get; set; }
    }
}
