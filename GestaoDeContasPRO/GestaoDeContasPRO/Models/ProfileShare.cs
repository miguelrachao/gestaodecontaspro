namespace GestaoDeContasPRO.Models
{
    public class ProfileShare
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public DateTime? DateLog { get; set; }
    }
}
