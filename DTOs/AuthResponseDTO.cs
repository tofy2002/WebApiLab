namespace Lab3.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime TokenExpiresAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
