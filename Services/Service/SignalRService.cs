using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.caching;

namespace Services.Service
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly ILogger<SignalRService> logger;

        public SignalRService(IHubContext<NotificationHub> _hubContext, ILogger<SignalRService> logger, IcacheServices cacheServices)
        {
            hubContext = _hubContext;
            this.logger = logger;
        }

        public async Task SendMessageAll(string message)
        {
            try
            {
                await hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"SignalRService ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }

        // Send message to all clients in a specific group
        public async Task SendMessageGroup(string GroupName, string message)
        {
            try
            {
                await hubContext.Clients.Group(GroupName).SendAsync("ReceiveMessage", message); // Consistent method name
            }
            catch (Exception ex)
            {
                logger.LogWarning($"SignalRService ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }

        // Send message to a specific user
        public async Task SendMessageUser(string UserID, string message)
        {
            try
            {
                await hubContext.Clients.User(UserID).SendAsync("ReceiveMessage", message); // Consistent method name

            }
            catch (Exception ex)
            {
                logger.LogWarning($"SignalRService ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }

        // Add user to group using connectionId
        public async Task AddUsertoGroup(string connectionId, string GroupName)
        {
            try
            {
                await hubContext.Groups.AddToGroupAsync(connectionId, GroupName);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"SignalRService ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }

        // Remove user from group using connectionId
        public async Task RemoveUserfromGroup(string connectionId, string GroupName)
        {
            try
            {
                await hubContext.Groups.RemoveFromGroupAsync(connectionId, GroupName);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"SignalRService ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }

    }
}
