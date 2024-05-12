using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using Video_conference_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
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

        public async Task SendMessage(string roomId, string senderId, string message)
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");
            string user = "undefined user";
            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson).Name;
            }
            await Clients.Groups(roomId).SendAsync("ReceiveMessage", message, user);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public async Task SendFile(string roomId, string senderId, string fileName, string fileData)
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");
            string user = "undefined user";
            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson).Name;
            }
            await Clients.Groups(roomId).SendAsync("ReceiveFile", fileName, fileData, user);
        }

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public async Task SendFile(string roomId, string senderId, string fileName, string fileData)
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");
            string user = "undefined user";
            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson).Name;
            }
            await Clients.Groups(roomId).SendAsync("ReceiveFile", fileName, fileData, user);
        }
    }
}