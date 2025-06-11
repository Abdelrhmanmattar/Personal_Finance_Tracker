namespace Core.DTO
{
    public class BudgetUserDTO
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public string Role { get; set; } // "Owner" or "Watcher"
    }

}
