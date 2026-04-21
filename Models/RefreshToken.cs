namespace Lab3.Models // Adjust namespace if needed
{
    public class RefreshToken
    {
        public int Id { get; set; } // Matches Id INT PRIMARY KEY
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;

        // Foreign Key to the User
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}