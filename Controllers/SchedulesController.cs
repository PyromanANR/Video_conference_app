using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Video_conference_app.Data;
using Video_conference_app.Models;

namespace Video_conference_app.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly Video_conference_appContext _context;

        public SchedulesController(Video_conference_appContext context)
        {
            _context = context;
        }

        //Index page to show all sceduled meetings for current user
        public IActionResult Index()
        {            
            var userData = HttpContext.Session.GetString("User");

            if (!string.IsNullOrEmpty(userData))
            {
                var user = JsonConvert.DeserializeObject<User>(userData);
                string name = user.Name;
                int id = user.Id;

                var currentTime = DateTime.Now;                
                var schedules = _context.Schedule.Where(s => s.OrganizerId == id && s.StartTime > currentTime).ToList();

                return View(schedules);
            }
            else
            {     
                //redirect to Sign In  page of user is not logged in
                return RedirectToAction("SignIn", "Users"); ;
            }
        }


        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }
        

        // GET: Schedules/Create        
        public IActionResult Create()
        {
            var userData = HttpContext.Session.GetString("User");

            if (!string.IsNullOrEmpty(userData))
            {
                var user = JsonConvert.DeserializeObject<User>(userData);
                int id = user.Id;

                ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", id);

                return View();
            }
            else
            {
                // Redirect to the Sign In page if the user is not logged in
                return RedirectToAction("SignIn", "Users");
            }
        }

        // POST: Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartTime")] Schedule schedule)
        {
            var userData = HttpContext.Session.GetString("User");

            if (!string.IsNullOrEmpty(userData))
            {
                var user = JsonConvert.DeserializeObject<User>(userData);
                int id = user.Id;
                schedule.OrganizerId = id;                
                User currentUser = await _context.User.FindAsync(id);
                schedule.Organizer = currentUser;

                schedule.OrganizerId = id;                
                schedule.Organizer = currentUser;
          
                _context.Add(schedule);                
                await _context.SaveChangesAsync();

                // Redirect to the Index action
                return RedirectToAction(nameof(Index));

                // If model state is not valid, return the view with the provided schedule
                ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", id);
                return View(schedule);
            }
            else
            {
                // Redirect to the Sign In page if the user is not logged in
                return RedirectToAction("SignIn", "Users");
            }
        }


        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            ViewData["OrganizerName"] = schedule.Organizer.Name;
            ViewData["OrganizerEmail"] = schedule.Organizer.Email;

            return View(schedule);
        }


        // POST: Schedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,StartTime")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            // Retrieve the existing schedule from the database
            var existingSchedule = await _context.Schedule
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            schedule.Organizer = existingSchedule.Organizer;
            schedule.OrganizerId = existingSchedule.OrganizerId;

            // Update the other properties with the new values
            existingSchedule.Title = schedule.Title;
            existingSchedule.StartTime = schedule.StartTime;

            
                try
                {
                    _context.Update(existingSchedule); // Update the existing schedule entity
                    await _context.SaveChangesAsync(); // Save changes to the database
                    return RedirectToAction(nameof(Index)); // Redirect to Index page after successful save
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(existingSchedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            // If ModelState is not valid, return the view with the existing schedule
            return View(existingSchedule);
        }


        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedule.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
