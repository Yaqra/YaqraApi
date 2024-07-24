using YaqraApi.Models.Enums;
using YaqraApi.Models;

namespace YaqraApi.DTOs.UserBookWithStatus
{
    public class AddUserBookWithStatusDto
    {
        public int BookId { get; set; }
        public UserBookStatus Status { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
