using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class FollowUserDto
    {
        [Required]
        public string FollowedUserId { get; set; }
    }
}
