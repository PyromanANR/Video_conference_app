using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<string, string> connectedUsers = new ConcurrentDictionary<string, string>();

        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task JoinRoom(string roomId, string userId)
        {
            connectedUsers.TryAdd(userId, await GetUserNameFromClient());
            Users.list.Add(Context.ConnectionId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Groups(roomId).SendAsync("user-connected", userId, await GetUserNameFromClient());
            await UpdateDropdown(roomId);
        }
        private async Task UpdateDropdown(string roomId)
        {
            var usersJson = JsonConvert.SerializeObject(connectedUsers);
            await Clients.Groups(roomId).SendAsync("update-dropdown", usersJson);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Users.list[Context.ConnectionId];
            connectedUsers.TryRemove(userId, out _);
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
        
        public async Task SendMessageToUser(string roomId, string senderId, string userId, string message)
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");
            string user = "undefined user";
            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson).Name;
            }
            await Clients.Groups(roomId).SendAsync("ReceivePersonalMessage", senderId, userId, message, user);
        }

        public async Task<string> GetUserNameFromClient()
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");
            string user = "undefined user";
            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson).Name;
            }
            return await Task.FromResult(user);
        }

        private Dictionary<string, bool> screenSharingStatus = new Dictionary<string, bool>();

        public async Task SetScreenSharingStatus(string roomId, string userId, bool isSharing)
        {
            screenSharingStatus[userId] = isSharing;
            await Clients.Groups(roomId).SendAsync("ScreenSharingStatusChanged", userId, isSharing);
        }

        public async Task<bool> GetScreenSharingStatus(string userId)
        {
            if (screenSharingStatus.ContainsKey(userId))
            {
                return await Task.FromResult(screenSharingStatus[userId]);
            }
            else
            {
                return await Task.FromResult(false);
            }
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