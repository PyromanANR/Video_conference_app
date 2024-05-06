using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
namespace Video_conference_app.Hubs
{

    public class RTCSessionDescriptionInit
    {
        public string Type { get; set; }
        public string Sdp { get; set; }
    }

    public class RTCIceCandidateInit
    {
        public string Candidate { get; set; }
        public string SdpMid { get; set; }
        public int SdpMLineIndex { get; set; }
    }


    public class ChatHub : Hub
    {
        //
        private static ConcurrentDictionary<string, List<string>> messageHistory = new ConcurrentDictionary<string, List<string>>();
        //

        public async Task SendMessage(string roomId, string user, string message)
        {
            //
            var fullMessage = $"{user}: {message}";
            //

            await Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);

            //
            messageHistory.AddOrUpdate(roomId, new List<string> { fullMessage }, (key, oldList) => {
                oldList.Add(fullMessage);
                return oldList;
            });
            //
        }

        public async Task JoinRoom(string roomId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, roomId);

            //
            if (messageHistory.TryGetValue(roomId, out var messages))
            {
                // Логування для перевірки того, що історія передається
                await Clients.Client(connectionId).SendAsync("ReceiveMessageHistory", messages);
            }
            //

        }

        public async Task LeaveRoom(string roomId)
        {
            // Get the current user's connection ID
            var connectionId = Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(connectionId, roomId);
        }


        public async Task ToggleMicrophone(string roomId, string user, bool isMicrophoneEnabled)
        {
            await Clients.Group(roomId).SendAsync("UpdateMicrophoneStatus", user, isMicrophoneEnabled);
        }

        public async Task SendOffer(string roomId, RTCSessionDescriptionInit offer)
        {
            // Forward the offer to the other peer(s) in the room
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string roomId, RTCSessionDescriptionInit answer)
        {
            // Forward the answer to the other peer(s) in the room
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIceCandidate(string roomId, RTCIceCandidateInit candidate)
        {
            // Forward the ICE candidate to the other peer(s) in the room
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveIceCandidate", candidate);
        }

    }
}
