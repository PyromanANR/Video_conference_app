using System;
using Microsoft.AspNetCore.Mvc;

namespace Video_conference_app.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            string roomId = Guid.NewGuid().ToString();

            return RedirectToAction("Room", new { roomId });
        }

        [HttpGet("/meeting/{roomId}")]
        public IActionResult Room(string roomId)
        {
            if (!Guid.TryParse(roomId, out _))
            {
                return BadRequest("Invalid room ID format.");
            }

            ViewBag.roomId = roomId;
            return View("HostView");
        }
    }
}