using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Specification;

namespace Services.Service
{
    public class NotificationServices : INotificationServices
    {
        private readonly IUnitOfWork unitOfWork;

        public NotificationServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public int AddNotification(Notifications notification)
        {

            unitOfWork.Repository<Notifications>().AddEntity(notification);
            return unitOfWork.SaveChanges();
        }
        public IReadOnlyList<NotificationDTO> GetAllNotifications(string ID)
        {
            var specs = new BaseSpecification<Notifications>(n => n.UserId == ID);
            return unitOfWork.Repository<Notifications>().FindAll(specs)
                .Select(x => new NotificationDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    CreatedAt = x.CreatedAt,
                    IsRead = x.IsRead,
                }
                ).ToList().AsReadOnly();
        }
        public int ReadNotification(int _idnot, string ID)
        {
            var specs = new BaseSpecification<Notifications>(n => (n.UserId == ID && n.Id == _idnot));
            var value = unitOfWork.Repository<Notifications>().Find(specs);
            if (value != null)
            {
                value.IsRead = true;
                unitOfWork.Repository<Notifications>().UpdateEntity(value);
                return unitOfWork.SaveChanges();
            }
            return -1;
        }
        public async Task<IReadOnlyList<Notifications>> addNotificationscommon(IEnumerable<string> IDs, string title, string message)
        {
            List<Notifications> notifications = new List<Notifications>();
            var date = DateTime.UtcNow;
            foreach (var id in IDs)
            {
                Notifications notification = new Notifications
                {
                    UserId = id,
                    Title = title,
                    Message = message,
                    Type = "Watcher",
                    CreatedAt = date,
                };
                notifications.Add(notification);
            }
            await unitOfWork.Repository<Notifications>().AddRangeAsync(notifications);
            var ret = unitOfWork.SaveChanges();
            return (ret > 0) ? notifications : null;
        }
    }
}
