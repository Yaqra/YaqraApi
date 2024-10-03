using System.ComponentModel.DataAnnotations.Schema;
using YaqraApi.Models;

namespace YaqraApi.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public int PostId { get; set; }
        public string Message { get; set; }
        public bool IsAck { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string PostType { get; set; }
    }
}
