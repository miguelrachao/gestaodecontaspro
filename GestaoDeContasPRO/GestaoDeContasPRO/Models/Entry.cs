namespace GestaoDeContasPRO.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public Category Category { get; set; } = new Category();
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}
