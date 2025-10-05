namespace GestaoDeContasPRO.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? OtpCode { get; set; }
        public DateTime? OtpExpiration { get; set; }
        public int? FavoriteProfileId { get; set; }
    }
}
