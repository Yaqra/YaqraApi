using YaqraApi.DTOs.Notification;
using YaqraApi.Models;

namespace YaqraApi.Services.IServices
{
    public interface INotificationService
    {
        Task Acknowledge(int notificationId);
        Task<List<NotificationDto>> GetAll(string userId, int page);
        Task<NotificationDto?> AddNotification(Notification notification);
        Task<Notification?> BuildNotification(int postId, string message, string receiverId);
    }
}
