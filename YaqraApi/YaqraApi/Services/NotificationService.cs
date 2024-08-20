using AutoMapper;
using Microsoft.EntityFrameworkCore;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Notification;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationContext _context;
        private readonly ICommunityRepository _communityRepository;
        private readonly Mapper _mapper;
        public NotificationService(ApplicationContext context, ICommunityRepository communityRepository)
        {
            _context = context;
            _communityRepository = communityRepository;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task Acknowledge(int notificationId)
        {
            var notification = await _context.Notifications.SingleOrDefaultAsync(n => n.Id == notificationId);
            if (notification == null)
                return;
            notification.IsAck = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<NotificationDto?> AddNotification(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            if (notification.Id == 0)
                return null;
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<Notification?> BuildNotification(int postId, string message, string receiverId)
        {
            var notification = new Notification
            {
                CreatedDate = DateTime.UtcNow,
                PostId = postId,
                ReceiverId = receiverId,
                Message = message,
            };
            await AddNotification(notification);

            return notification;
        }

        public async Task<List<NotificationDto>> GetAll(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var notifications = _context.Notifications
                .Where(n=>n.ReceiverId == userId)
                .OrderByDescending(n=>n.Id)
                .Skip((page-1)*Pagination.Notifications).Take(Pagination.Notifications)
                .Select(n=> _mapper.Map<NotificationDto>(n))
                .ToList();
            return notifications;
        }
    }
}
