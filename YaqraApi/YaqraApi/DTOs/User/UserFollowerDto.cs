using YaqraApi.Models;

namespace YaqraApi.DTOs.User
{
    public class UserFollowerDto
    {
        public ApplicationUser Followed { get; set; }
        public ApplicationUser Follower { get; set; }
    }
}
