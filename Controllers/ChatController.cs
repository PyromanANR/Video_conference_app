using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult HostChat()
        {
            var hostId = Guid.NewGuid().ToString();
            var requestScheme = Request.Scheme; // Get the current request scheme
            ViewBag.HostId = hostId;
            ViewBag.RequestScheme = requestScheme; // Pass the request scheme to the view
            return View("HostView");
        }

        public IActionResult JoinChat(string hostId)
        {
            ViewBag.HostId = hostId;
            return View("JoinView");
        }
    }
}
