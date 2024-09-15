using System.ComponentModel.DataAnnotations.Schema;

namespace YaqraApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Receiver))]
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Message { get; set; }
        public bool IsAck { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
