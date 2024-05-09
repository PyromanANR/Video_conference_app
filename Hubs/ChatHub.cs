using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
namespace Video_conference_app.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomId, string userId)
        {
            Users.list.Add(Context.ConnectionId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Groups(roomId).SendAsync("user-connected", userId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);
            return base.OnDisconnectedAsync(exception);
        }
    
        // Add this method to your SignalR hub class
        public async Task SendMessage(string roomId, string senderId, string message)
        {
            await Clients.Groups(roomId).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
