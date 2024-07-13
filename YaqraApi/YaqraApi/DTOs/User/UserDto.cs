namespace YaqraApi.DTOs.User
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfileCover { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
    }
}
