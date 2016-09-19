using System.Linq;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace GctgsWeb.Controllers
{
    public class BoardGamesController : Controller
    {
        private GctgsContext _context;

        public BoardGamesController(GctgsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.BoardGames.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BoardGame boardGame)
        {
            if (ModelState.IsValid)
            {
                _context.BoardGames.Add(boardGame);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(boardGame);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
