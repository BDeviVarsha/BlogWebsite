using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BlogWebsite.Hubs
{
    public class ChatHub:Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserRepository _userRepository;

        // userId → connectionId mapping
        private static ConcurrentDictionary<int, string> _connections = new();

        public ChatHub(IChatService chatService, IUserRepository userRepository)
        {
            _chatService = chatService;
            _userRepository = userRepository;
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
                await Clients.Client(receiverConn).SendAsync("ReceiveMessage", message);
            }
        }

        // TYPING EVENT (matches Angular)
        public async Task Typing(int senderId, int receiverId)
        {
            if (_connections.TryGetValue(receiverId, out var receiverConn))
            {
                await Clients.Client(receiverConn).SendAsync("UserTyping", senderId);
            }
        }

        // SEND USERS LIST
        private async Task SendUsersList()
        {
            var users = await _userRepository.GetAllUsersAsync();
            await Clients.All.SendAsync("ReceiveUsers", users);
        }
        //public async Task MarkAsSeen(int senderId, int receiverId)
        //{
        //    await _chatService.MarkAsSeenAsync(senderId, receiverId);
        //    // Notify sender that messages have been seen
        //    if (_connections.TryGetValue(senderId, out var senderConn))
        //    {
        //        await Clients.Client(senderConn).SendAsync("MessagesSeen", receiverId);
        //    }
        //}
        public async Task MarkAsSeen(int senderId, int receiverId)
        {
            //Console.WriteLine($"MarkAsSeen called: {senderId} -> {receiverId}");
            var seenMessageIds = await _chatService.MarkAsSeenAsync(senderId, receiverId);

            // Notify sender (messages were seen)
            if (_connections.TryGetValue(senderId, out var senderConn))
            {
                await Clients.Client(senderConn).SendAsync("MessagesSeen", seenMessageIds);
            }
        }
    }
}
