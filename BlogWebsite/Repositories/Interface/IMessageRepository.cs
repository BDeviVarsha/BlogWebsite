using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IMessageRepository
    {
        Task<int> SaveMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2);
        Task<List<int>> MarkAsSeenAsync(int senderId, int receiverId);
    }
}
