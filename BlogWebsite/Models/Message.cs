namespace BlogWebsite.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int Sender_Id { get; set; }
        public int Receiver_Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Seen { get; set; } = false;
    }
}
