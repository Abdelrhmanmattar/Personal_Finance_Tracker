using Core.entities;
using Core.Interfaces;
using Core.Specification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.BackgroundServices
{
    public class CleanNotification : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CleanNotification> _logger;

        public CleanNotification(IServiceProvider serviceProvider, ILogger<CleanNotification> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var specs = new BaseSpecification<Notifications>(n =>
                        n.CreatedAt <= DateTime.UtcNow.AddDays(-7));

                    var oldNotifications = unitOfWork.Repository<Notifications>().FindAll(specs);

                    if (oldNotifications.Any())
                    {
                        foreach (var item in oldNotifications)
                        {
                            unitOfWork.Repository<Notifications>().DeleteEntity(item.Id);
                        }

                        unitOfWork.SaveChanges();
                        _logger.LogInformation("Old notifications cleaned successfully.");
                        await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during notification cleanup.");
                }
            }
        }
    }
}
