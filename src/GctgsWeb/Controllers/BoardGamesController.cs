using System.Linq;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GctgsWeb.Controllers
{
    [Authorize("RegisteredUser")]
    public class BoardGamesController : Controller
    {
        private readonly GctgsContext _context;
        private readonly IMemoryCache _memoryCache;

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

        [HttpGet("boardgames/{id}")]
        public IActionResult BoardGame(int id)
        {
            var result = _context.BoardGames
                .Where(boardGame => boardGame.Id == id)
                .Include(boardGame => boardGame.Owner)
                .SingleOrDefault();

            if (result == null) return new NotFoundResult();

            if (_context.IsAdmin(User))
            {
                ViewData["Admin"] = true;
                return View("Edit", result);
            }
            if (User.Identity.Name == result.Owner.Crsid)
            {
                ViewData["Admin"] = false;
                return View("Edit", result);
            }
            return View(result);
        }

        [HttpPost("boardgames/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BoardGame updatedBoardGame)
        {
            if (ModelState.IsValid)
            {
                var result = _context.BoardGames
                    .Where(boardGame => boardGame.Id == id)
                    .Include(boardGame => boardGame.Owner)
                    .SingleOrDefault();

                if ((result.Owner.Crsid != User.Identity.Name) &&
                    !_context.IsAdmin(User))
                    return Unauthorized();

                if (_context.IsAdmin(User)) result.Name = updatedBoardGame.Name;
                result.Location = updatedBoardGame.Location;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(updatedBoardGame);
        }

        [HttpPost("boardgames/delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var result = _context.BoardGames
                .Where(boardGame => boardGame.Id == id)
                .Include(boardGame => boardGame.Owner)
                .SingleOrDefault();

            if ((result.Owner.Crsid != User.Identity.Name) &&
                !_context.IsAdmin(User))
                return Unauthorized();

            _context.BoardGames.Remove(result);
            _context.SaveChanges();

            return RedirectToAction("Index");
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

        [HttpGet("boardgames/create")]
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
