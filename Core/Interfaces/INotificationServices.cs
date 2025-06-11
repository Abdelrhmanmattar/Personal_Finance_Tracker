using Core.DTO;
using Core.entities;

namespace Core.Interfaces
{
    public interface INotificationServices
    {
        int AddNotification(Notifications notification);
        IReadOnlyList<NotificationDTO> GetAllNotifications(string ID);
        int ReadNotification(int notificationId, string userId);
        Task<IReadOnlyList<Notifications>> addNotificationscommon(IEnumerable<string> IDs, string title, string message);
    }


}
