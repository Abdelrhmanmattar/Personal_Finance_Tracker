namespace Core.entities
{
    public class Income //دخل
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public string User_Id { get; set; }
        public Users_App User_App { get; set; }
        public DateTime Date_Deposite { get; set; }
        public override string ToString()
        {
            return $"[ {Id} , {Amount} ]";
        }
    }
}
