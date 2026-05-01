namespace BlogWebsite.DTOs
{
    public class PostDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Files { get; set; }
        public int? UserId { get; set; } = null;
    }
}
