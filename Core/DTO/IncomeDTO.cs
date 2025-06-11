namespace Core.DTO
{
    public class IncomeDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public DateTime Date_Deposite { get; set; }
        public override string ToString()
        {
            return $"[{Id}] for Source:{Source} with amount = {Amount} in {Date_Deposite}";
        }
    }
}
