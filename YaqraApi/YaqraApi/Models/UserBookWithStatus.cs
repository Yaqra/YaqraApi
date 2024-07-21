using YaqraApi.Models.Enums;

namespace YaqraApi.Models
{
    public class UserBookWithStatus
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime AddedDate { get; set; }
        public UserBookStatus Status { get; set; }
    }
}
