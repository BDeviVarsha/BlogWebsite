namespace BlogWebsite.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Is_Pinned { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
        public string? Created_by { get; set; }
    }
}
