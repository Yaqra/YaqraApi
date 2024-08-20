namespace YaqraApi.Models
{
    public class PostLikes
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
