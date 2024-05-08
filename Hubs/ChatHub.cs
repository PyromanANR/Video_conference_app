using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
namespace Video_conference_app.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, List<string>> messageHistory = new ConcurrentDictionary<string, List<string>>();

        public async Task SendMessage(string roomId, string user, string message)
        {
            var fullMessage = $"{user}: {message}";

            await Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);

            messageHistory.AddOrUpdate(roomId, new List<string> { fullMessage }, (key, oldList) => {
                oldList.Add(fullMessage);
                return oldList;
            });
        }

        public async Task JoinRoom(string roomId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, roomId);

            if (messageHistory.TryGetValue(roomId, out var messages))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessageHistory", messages);
            }
            
        }

        public async Task LeaveRoom(string roomId)
        {
            var connectionId = Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(connectionId, roomId);
        }
        
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
