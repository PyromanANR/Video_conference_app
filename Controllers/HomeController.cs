using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Video_conference_app.Models;

namespace Video_conference_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
