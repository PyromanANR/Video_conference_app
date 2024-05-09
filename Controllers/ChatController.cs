using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Controllers
{
    public class ChatController : Controller
    {

        public IActionResult Index()
        {
            return View("JoinView");
        }

        public IActionResult HostChat()
        {
            string roomId = Guid.NewGuid().ToString();
            var joinChatLink = Url.Action("JoinChat", "Chat", new { roomId }, Request.Scheme);
            ViewBag.JoinChatLink = joinChatLink;
    
            return RedirectToAction("JoinChat", "Chat", new { roomId });
        }


        public IActionResult JoinChat(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return BadRequest("Invalid room ID");
            }

            ViewBag.RoomId = roomId;

            return View("HostView");
        }

    }
}
