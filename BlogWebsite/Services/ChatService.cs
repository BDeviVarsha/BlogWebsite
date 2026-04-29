using BlogWebsite.Models;
using BlogWebsite.Repositories.Implementation;
using BlogWebsite.Repositories.Interface;
//using Renci.SshNet.Messages;

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
                Sender_Id = senderId,
                Receiver_Id = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                Seen = false
            };

            message.Id = await _repo.SaveMessageAsync(message);

            return message;
        }
        public async Task<List<Message>> GetMessagesAsync(int user1, int user2)
        {
            var messages = await _repo.GetMessagesAsync(user1, user2);
            return messages.ToList();
        }
        public async Task MarkAsSeenAsync(int senderId, int receiverId)
        {
            await _repo.MarkAsSeenAsync(senderId, receiverId);
        }
    }
}
