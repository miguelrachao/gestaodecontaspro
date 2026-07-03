namespace GestaoDeContasPRO.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ActionType Type { get; set; }
        public double Budget { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public bool Active { get; set; }
    }
}
