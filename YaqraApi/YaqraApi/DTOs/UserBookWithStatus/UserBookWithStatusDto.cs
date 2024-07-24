using YaqraApi.Models.Enums;
using YaqraApi.Models;

namespace YaqraApi.DTOs.UserBookWithStatus
{
    public class UserBookWithStatusDto
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public UserBookStatus Status { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
