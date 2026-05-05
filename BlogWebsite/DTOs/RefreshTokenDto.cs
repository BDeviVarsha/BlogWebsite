namespace BlogWebsite.DTOs
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
