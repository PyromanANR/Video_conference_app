using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult HostChat()
        {
            string roomId = Guid.NewGuid().ToString();
            return RedirectToAction("JoinChat", "Chat", new { roomId });
        }

        public IActionResult JoinChat(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return BadRequest("Invalid room ID");
            }

            var joinChatLink = Url.Action("JoinChat", "Chat", new { roomId }, Request.Scheme);

            ViewBag.RoomId = roomId;
            ViewBag.JoinChatLink = joinChatLink;

            return View("HostView");
        }

    }
}
