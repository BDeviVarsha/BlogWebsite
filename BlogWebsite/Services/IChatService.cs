using BlogWebsite.Models;

namespace BlogWebsite.Services
{
    public interface IChatService
    {
        Task<Message> SendMessageAsync(int senderId, int receiverId, string content);
        Task<List<Message>> GetMessagesAsync(int user1, int user2);
        Task<List<int>> MarkAsSeenAsync(int senderId, int receiverId);
    }
}
