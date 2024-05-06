using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // GET: Schedules
        /*public async Task<IActionResult> Index()
        {
            var video_conference_appContext = _context.Schedule.Include(s => s.Organizer);
            return View(await video_conference_appContext.ToListAsync());
        }*/

        //Index page to show all sceduled meetings 
        public IActionResult Index()
        {
            int userId = 1;
            var currentTime = DateTime.Now;
            var schedules = _context.Schedule.Where(s => s.OrganizerId == userId && s.StartTime > currentTime).ToList();
            return View(schedules);
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

        /*// GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartTime,OrganizerId")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", schedule.OrganizerId);
            return View(schedule);
        }*/


        // GET: Schedules/Create
        public IActionResult Create()
        {
            // Get the current user's id
            int currentUserId = 1;

            // Pass the current user's id to the view
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", currentUserId);

            return View();
        }

        // POST: Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartTime")] Schedule schedule)
        {
            // Get the current user's id
            int currentUserId = 1; // Replace with your logic to get the current user's id

            // Retrieve the user from the User table using the current user's id
            User currentUser = await _context.User.FindAsync(currentUserId);

            if (ModelState.IsValid)
            {
                // Set the OrganizerId property of the schedule
                schedule.OrganizerId = currentUserId;

                // Assign the retrieved user to the Organizer navigation property of the schedule
                schedule.Organizer = currentUser;

                // Add the schedule to the context
                _context.Add(schedule);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, return the view with the provided schedule
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", currentUserId);
            return View(schedule);
        }



        // GET: Schedules/Edit/5
        /*public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", schedule.OrganizerId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,StartTime,OrganizerId")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizerId"] = new SelectList(_context.User, "Id", "Email", schedule.OrganizerId);
            return View(schedule);
        }*/

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
