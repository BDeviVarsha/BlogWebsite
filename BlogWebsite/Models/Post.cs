using System.Net.Mail;

namespace BlogWebsite.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
    }
}
