using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class UsernameUpdateDto
    {
        [Required]
        public string Username { get; set; }
    }
}
