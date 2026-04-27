using BlogWebsite.Models;

namespace BlogWebsite.Services
{
    public interface IChatService
    {
        Task<Message> SendMessageAsync(int senderId, int receiverId, string content);
    }
}
