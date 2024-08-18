﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using YaqraApi.Helpers;
using YaqraApi.Hubs;
using YaqraApi.Services.IServices;

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hub)
        {
            _notificationService = notificationService;
            _hub = hub;
        }
        [HttpPut]
        public async Task Acknowledge([FromQuery] int notificationId)
        {
            await _notificationService.Acknowledge(notificationId);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] int page)
        {
            var notifications = await _notificationService.GetAll(UserHelpers.GetUserId(User), page);
            if (notifications == null)
                notifications = new List<DTOs.Notification.NotificationDto>();
            return Ok(notifications);
        }
    }
}
