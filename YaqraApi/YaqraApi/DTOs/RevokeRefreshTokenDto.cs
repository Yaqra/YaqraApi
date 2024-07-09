using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs
{
    public class RevokeRefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
