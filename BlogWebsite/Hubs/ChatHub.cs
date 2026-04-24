using BlogWebsite.Models;
using BlogWebsite.Repositories.Implementation;
using Microsoft.AspNetCore.SignalR;

namespace BlogWebsite.Hubs
{
    public class ChatHub:Hub
    {
        private readonly MessageRepository _repository;

        public ChatHub(MessageRepository repository)
        {
            _repository = repository;
        }

        public async Task SendMessage(int senderId, int receiverId, string content)
        {
            var userIdString = Context.GetHttpContext().Request.Query["userId"];
            if (!int.TryParse(userIdString, out int userId))
            {
                throw new HubException("Invalid userId");
            }

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            var savedMessage = await _repository.SendMessage(message);

            // Send to all clients or just to the receiver
            await Clients.User(receiverId.ToString())
                         .SendAsync("ReceiveMessage", savedMessage);
            await Clients.Caller.SendAsync("ReceiveMessage", savedMessage); // also send to sender
        }

        public async Task Typing(int senderId, int receiverId)
        {
            // Notify the receiver that the sender is typing
            //await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId);
            try
            {
                await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId);
            }
            catch (Exception ex)
            {
                throw new HubException($"Typing failed: {ex.Message} | {ex.InnerException?.Message}");
            }
        }


        //public async Task SendPrivateMessage(string userId, string message)
        //{
        //    await Clients.User(userId).SendAsync("ReceiveMessage", message);
        //}

        //public async Task SendGroupMessage(string groupName, string message)
        //{
        //    await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        //}

        //public async Task SendBroadcastMessage(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}

        //public async Task JoinGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //}

        //public async Task LeaveGroup(string groupName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        //}
    }
}
