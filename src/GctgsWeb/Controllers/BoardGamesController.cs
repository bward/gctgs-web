using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GctgsWeb.Controllers
{
    public class BoardGamesController : Controller
    {
        private readonly GctgsContext _context;
        private readonly  IMemoryCache _memoryCache;

        public BoardGamesController(GctgsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
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
