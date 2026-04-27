using BlogWebsite.Models;
using BlogWebsite.Repositories.Implementation;
using BlogWebsite.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BlogWebsite.Hubs
{
    public class ChatHub:Hub
    {
        private readonly IChatService _chatService;

        // userId → connectionId mapping
        private static ConcurrentDictionary<int, string> _connections = new();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // On connect → store user
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = int.Parse(httpContext.Request.Query["userId"]);

            _connections[userId] = Context.ConnectionId;

            await SendUsersList();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId);

            if (!user.Equals(default(KeyValuePair<int, string>)))
            {
                _connections.TryRemove(user.Key, out _);
            }

            await SendUsersList();
            await base.OnDisconnectedAsync(exception);
        }

        // SEND MESSAGE (matches Angular)
        public async Task SendMessage(int senderId, int receiverId, string content)
        {
            var message = await _chatService.SendMessageAsync(senderId, receiverId, content);

            // Send to receiver
            if (_connections.TryGetValue(receiverId, out var receiverConn))
            {
                await Clients.Client(receiverConn)
                    .SendAsync("ReceiveMessage", message);
            }

            // Send back to sender
            if (_connections.TryGetValue(senderId, out var senderConn))
            {
                await Clients.Client(senderConn)
                    .SendAsync("ReceiveMessage", message);
            }
        }

        // TYPING EVENT (matches Angular)
        public async Task Typing(int senderId, int receiverId)
        {
            if (_connections.TryGetValue(receiverId, out var receiverConn))
            {
                await Clients.Client(receiverConn)
                    .SendAsync("UserTyping", senderId);
            }
        }

        // SEND USERS LIST
        private async Task SendUsersList()
        {
            var users = _connections.Keys.Select(id => new
            {
                Id = id,
                Name = $"User {id}",
                IsOnline = true
            });

            await Clients.All.SendAsync("ReceiveUsers", users);
        }



        //private readonly MessageRepository _repository;

        //public ChatHub(MessageRepository repository)
        //{
        //    _repository = repository;
        //}

        //public async Task SendMessage(int senderId, int receiverId, string content)
        //{
        //    var userIdString = Context.GetHttpContext().Request.Query["userId"];
        //    if (!int.TryParse(userIdString, out int userId))
        //    {
        //        throw new HubException("Invalid userId");
        //    }

        //    var message = new Message
        //    {
        //        SenderId = senderId,
        //        ReceiverId = receiverId,
        //        Content = content,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    var savedMessage = await _repository.SaveMessageAsync(message);

        //    // Send to all clients or just to the receiver
        //    await Clients.User(receiverId.ToString())
        //                 .SendAsync("ReceiveMessage", savedMessage);
        //    await Clients.Caller.SendAsync("ReceiveMessage", savedMessage); // also send to sender
        //}

        //public async Task Typing(int senderId, int receiverId)
        //{
        //    // Notify the receiver that the sender is typing
        //    //await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId);
        //    try
        //    {
        //        await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HubException($"Typing failed: {ex.Message} | {ex.InnerException?.Message}");
        //    }
        //}


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
