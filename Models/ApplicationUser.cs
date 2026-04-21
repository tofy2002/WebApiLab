using Microsoft.AspNetCore.Identity;

namespace Lab3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public int age { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
