using Core.entities;
using Core.Specification;
using Microsoft.Extensions.Hosting;
using Repository.MODELS.DATA;
using Repository.UnitofWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackgroundServices
{
    public class CleanNotification : BackgroundService
    {
        private readonly UnitOfWork _unitOfWork;

        public CleanNotification(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);

            var specs = new BaseSpecification<Notifications>(n => n.CreatedAt <= DateTime.UtcNow.AddDays(-7));
            var oldNotifications = _unitOfWork.Repository<Notifications>().FindAll(specs);

            if(oldNotifications.Any())
            {
                foreach(var item in oldNotifications)
                {
                    _unitOfWork.Repository<Notifications>().DeleteEntity(item.Id);
                }
                _unitOfWork.SaveChanges();
            }
        }
    }
}
