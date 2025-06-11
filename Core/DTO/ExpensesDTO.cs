namespace Core.DTO
{
    public class ExpensesDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int BudgetId { get; set; }

        public DateTime Date_Withdraw { get; set; }
    }
}
