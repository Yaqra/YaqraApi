using Microsoft.AspNetCore.Identity;

namespace YaqraApi.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfileCover { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
