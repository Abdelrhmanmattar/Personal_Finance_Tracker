using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.caching;

namespace Services.Service
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IcacheServices _cacheServices;
        private readonly IBudget budgetRepo;
        private readonly IBudgetUser budgetUserRepo;

        public NotificationHub(ILogger<NotificationHub> logger, IcacheServices cacheServices, IBudget budgetRepo, IBudgetUser budgetUserRepo)
        {
            _logger = logger;
            _cacheServices = cacheServices;
            this.budgetRepo = budgetRepo;
            this.budgetUserRepo = budgetUserRepo;
        }
        public async Task Sendmessage(string message)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"NotificationHub ---> {DateTime.UtcNow} \n{ex.Message} ");
            }
        }
    }
}
