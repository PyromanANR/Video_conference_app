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
    public class UsersController : Controller
    {
        private readonly Video_conference_appContext _context;

        public UsersController(Video_conference_appContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] User user, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (ConfirmPassword == user.Password)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Account created successfully!";
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                TempData["Error"] = "Passwords have to equal";
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["Info"] = "Account was successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Clear();
            TempData["Info"] = "Account was successfully deleted!";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
        
        // GET: Users/SignIn
        public IActionResult SignIn()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([Bind("Email,Password")] User user)
        {
            var activeUser = await _context.User.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

            if (activeUser != null)
            {
                TempData["Success"] = "Log in successful!";
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(activeUser));
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                TempData["Error"] = "Incorrect email or password!";
            }

            return View(user);
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            TempData["Info"] = "You are logged out of your account!";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
