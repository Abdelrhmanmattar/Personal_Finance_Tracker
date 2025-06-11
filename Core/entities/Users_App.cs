using Microsoft.AspNetCore.Identity;
namespace Core.entities
{
    public class Users_App : IdentityUser
    {
        public DateTime Created_At { get; set; }
        public List<Budgets> Budgets { get; set; }
        public List<Income> Incomes { get; set; }
        public List<Expenses> Expenses { get; set; }
        public List<Category> Categories { get; set; }
        public List<Notifications> Notifications { get; set; }
        public List<BudgetUser> BudgetUsers { get; set; }
    }
}
