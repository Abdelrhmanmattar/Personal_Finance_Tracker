namespace Core.entities
{
    public class Expenses //صرف
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int BudgetId { get; set; }
        public string User_Id { get; set; }
        public Users_App User_App { get; set; }
        public Budgets Budget { get; set; }
        public DateTime Date_Withdraw { get; set; }
        public override string ToString()
        {
            return $"[{Id},{Amount}]";
        }
    }
}
