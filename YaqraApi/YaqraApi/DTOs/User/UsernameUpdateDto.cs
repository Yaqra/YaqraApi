using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class UsernameDto
    {
        [Required]
        public string Username { get; set; }
    }
}
