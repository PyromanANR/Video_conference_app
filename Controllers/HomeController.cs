using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using Video_conference_app.Data;
using Video_conference_app.Models;

namespace Video_conference_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Video_conference_appContext _context;

        /*public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/
        
        public HomeController(ILogger<HomeController> logger, Video_conference_appContext context)
        {
            _logger = logger;
            _context = context;
        }

        /*public IActionResult Index()
        {
            return View();
        }*/


        public IActionResult Index()
        {
            var userData = HttpContext.Session.GetString("User");

            if (!string.IsNullOrEmpty(userData))
            {
                var user = JsonConvert.DeserializeObject<User>(userData);
                int id = user.Id;

                var currentTime = DateTime.Now;

                // Retrieve the most close meeting for the current user
                var closestMeeting = _context.Schedule
                    .Where(s => s.OrganizerId == id && s.StartTime > currentTime)
                    .OrderBy(s => s.StartTime) 
                    .FirstOrDefault(); 

                return View(closestMeeting); // Pass the closest meeting to the view
            }
            else
            {
                // If user is not logged in or has no meetings, return the view without any data
                return View(null);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
