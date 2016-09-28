using System;
using System.Collections.Generic;
using GctgsWeb.Models;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            var key = Request.Headers["X-GCTGS-Key"];
            if (!_context.Users.Any(user => user.Key == key))
            {
                return StatusCode(403);
            }

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

        [HttpGet("authenticate")]
        [Authorize("RegisteredUser")]
        public IActionResult Authenticate()
        {
            var data = JsonConvert.SerializeObject(_context.Users.SingleOrDefault(user => user.Crsid == User.Identity.Name),
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()});
            return new RedirectResult("gctgs://authenticate?data=" + Uri.EscapeDataString(data), false);
        }
    }
}
