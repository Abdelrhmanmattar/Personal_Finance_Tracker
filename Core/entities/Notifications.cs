namespace Core.entities
{
    public class Notifications
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public Users_App User_App { get; set; }
        public Notifications() { }

        // You can manually set properties as needed
        public void Initialize(string? userId, string? title, string message, string type, DateTime? createdAt = null, bool isRead = false)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            IsRead = isRead;
        }
    }

}
