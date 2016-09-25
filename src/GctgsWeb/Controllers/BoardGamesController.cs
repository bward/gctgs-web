using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace GctgsWeb.Controllers
{
    [Authorize("RegisteredUser")]
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
            return View(_context.BoardGames
                .Include(boardGame => boardGame.Owner)
                .OrderBy(boardGame => boardGame.Name)
                .ToList());
        }

        [HttpGet("boardgames/boardgame/{id}")]
        public IActionResult BoardGame(int id)
        {
            var result = _context.BoardGames
                .Where(boardGame => boardGame.Id == id)
                .Include(boardGame => boardGame.Owner)
                .SingleOrDefault();

            return result != null ? View(result) : (ActionResult) new NotFoundResult();
        }

        [HttpGet("boardgames/locations/{location}")]
        public IActionResult Locations(string location)
        {
            return View("Index", _context.BoardGames
                .Include(boardGame => boardGame.Owner)
                .Where(boardGame => boardGame.Location == location)
                .OrderBy(boardGame => boardGame.Name)
                .ToList());
        }

        [HttpGet("boardgames/owners/{owner}")]
        public IActionResult Owners(string owner)
        {
            return View("Index", _context.BoardGames
                .Include(boardGame => boardGame.Owner)
                .Where(boardGame => boardGame.Owner.Crsid == owner)
                .OrderBy(boardGame => boardGame.Name)
                .ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BoardGame boardGame)
        {
            boardGame.Owner = _context.Users.Single(u => u.Crsid == User.Identity.Name);

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
