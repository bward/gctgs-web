using System.Collections.Generic;
using GctgsWeb.Models;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace GctgsWeb.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly GctgsContext _context;
        private readonly IMemoryCache _memoryCache;

        public ApiController(GctgsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpGet("boardgames")]
        public IActionResult BoardGames()
        {
            var boardGames = _context.BoardGames
                .Include(boardGame => boardGame.Owner)
                .OrderBy(boardGame => boardGame.Name).ToList();

            return new ObjectResult(boardGames.Select(async boardGame =>
            {
                boardGame.BggDetails = await boardGame.GetBggDetails(_memoryCache);
                return boardGame;
            })
            .Select(boardGame => boardGame.Result));
        }

        
    }
}
