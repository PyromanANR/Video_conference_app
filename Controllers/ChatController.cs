using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult HostChat()
        {
            // Generate a unique room ID
            string roomId = Guid.NewGuid().ToString();
            return RedirectToAction("JoinChat", "Chat", new { roomId });
        }

        public IActionResult JoinChat(string roomId)
        {
            // Validate the roomId parameter
            if (string.IsNullOrEmpty(roomId))
            {
                return BadRequest("Invalid room ID");
            }

            // Generate a link for the client to join the chat room
            var joinChatLink = Url.Action("JoinChat", "Chat", new { roomId }, Request.Scheme);

            // You can pass additional data to the view if needed
            ViewBag.RoomId = roomId;
            ViewBag.JoinChatLink = joinChatLink;

            return View("HostView");
        }

    }
}
