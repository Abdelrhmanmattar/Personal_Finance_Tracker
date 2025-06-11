namespace Core.entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string User_Id { get; set; }
        public Users_App User_App { get; set; }
        public List<Budgets> Budgets { get; set; }
        /// <summary>
        ///public List<Expenses> Expenses { get; set; }
        ///public List<Budgets> Budgets { get; set; }
        /// </summary>
    }



}
