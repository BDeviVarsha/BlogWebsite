using BlogWebsite.Models;
using BlogWebsite.Repositories.Implementation;
using BlogWebsite.Repositories.Interface;

namespace BlogWebsite.Services
{
    public class ChatService:IChatService
    {
        private readonly IMessageRepository _repo;

        public ChatService(IMessageRepository repo)
        {
            _repo = repo;
        }

        public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                Seen = false
            };

            message.Id = await _repo.SaveMessageAsync(message);

            return message;
        }

    }
}
