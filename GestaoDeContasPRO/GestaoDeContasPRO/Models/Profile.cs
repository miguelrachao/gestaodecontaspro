namespace GestaoDeContasPRO.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool Active { get; set; }
    }
}
