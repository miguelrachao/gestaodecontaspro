namespace GestaoDeContasPRO.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool Favorite { get; set; }
        public bool Active { get; set; }
    }
}
