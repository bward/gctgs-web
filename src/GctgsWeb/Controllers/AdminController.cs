using System.Linq;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace GctgsWeb.Controllers
{
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
            ViewData["users"] = users;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
