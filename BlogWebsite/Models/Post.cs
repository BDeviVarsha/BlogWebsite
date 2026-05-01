namespace BlogWebsite.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string Status { get; set; }
        public string? RejectedReason { get; set; }
        public int? UserId { get; set; }
        public bool is_deleted { get; set; }
        public string? Deleted_Reason { get; set; }
    }
}
