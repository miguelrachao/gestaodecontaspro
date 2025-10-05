namespace GestaoDeContasPRO.Models
{
    public class ProfileTransaction
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public int UserId { get; set; }
    }
}
