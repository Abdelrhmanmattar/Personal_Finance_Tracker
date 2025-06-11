namespace Core.entities
{
    public class Budgets
    {
        public int Id { get; set; }
        public decimal LimitAmount { get; set; }
        public int Cat_Id { get; set; }
        public Category Category { get; set; }
        public string User_Id { get; set; }
        public Users_App User_App { get; set; }
        public List<BudgetUser> BudgetUsers { get; set; }
        public List<Expenses> Expenses { get; set; }
    }
}
