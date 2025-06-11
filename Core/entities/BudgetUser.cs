namespace Core.entities
{
    public class BudgetUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Users_App User { get; set; }
        public int BudgetId { get; set; }
        public Budgets Budget { get; set; }
        public string Role { get; set; } // "Owner" or "Watcher"
    }
}
