using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using YaqraApi.Services.IServices;

namespace YaqraApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public NotificationHub(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }
        public override async Task OnConnectedAsync()
        {
            if(Context.UserIdentifier == null)
                await Console.Out.WriteLineAsync("UserIdentifier is null");
            else
                await Console.Out.WriteLineAsync(Context.UserIdentifier);

            await Console.Out.WriteLineAsync($"conId: {Context.ConnectionId}");

            await _userService.AddConnectionIdToUser(Context.UserIdentifier, Context.ConnectionId);
            var notifications = await _notificationService.GetAll(Context.UserIdentifier, 1);
            foreach (var notification in notifications)
            {
                await Clients.Caller.SendAsync("ReceiveNotification", notification);
            }
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _userService.RemoveConnectionIdFromUser(Context.UserIdentifier, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
