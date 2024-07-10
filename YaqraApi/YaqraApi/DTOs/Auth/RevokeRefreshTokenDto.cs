using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.Auth
{
    public class RevokeRefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
