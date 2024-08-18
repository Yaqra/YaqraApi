using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace YaqraApi.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfileCover { get; set; }
        public ICollection<ApplicationUser> Followings { get; set; } //i follow them
        public ICollection<ApplicationUser> Followers { get; set; } // they follow me
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Genre> FavouriteGenres { get; set; }
        public ICollection<Author> FavouriteAuthors { get; set; }
        public ICollection<ReadingGoal> ReadingGoals { get; set; }
        public ICollection<UserBookWithStatus> UserBooks { get; set; }
        public ICollection<Comment> Comments{ get; set; }
        public ICollection<RecommendationStatistics> RecommendationStatistics { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Connection> Connections { get; set; }
    }
    [Owned]
    public class Connection
    {
        public string ConnectionId { get; set;}
    }
}
