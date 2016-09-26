using System;
using System.Linq;
using System.Security.Cryptography;
using GctgsWeb.Models;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GctgsWeb.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        private readonly GctgsContext _context;

        public AdminController(GctgsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var keyData = new byte[32];
                rng.GetBytes(keyData);
                user.Key = Convert.ToBase64String(keyData);
                if (ModelState.IsValid)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
