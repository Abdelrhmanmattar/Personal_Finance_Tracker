namespace Core.DTO
{
    public class ReportSummaryDTO
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public string TopExpenseCategory { get; set; }
        public string TopIncomeSource { get; set; }
        public string BudgetStatus { get; set; }
    }
}
