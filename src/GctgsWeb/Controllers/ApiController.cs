using System;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GctgsWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GctgsWeb.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly GctgsContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly EmailSettings _emailSettings;
        private readonly FirebaseSettings _firebaseSettings;

        public ApiController(GctgsContext context, IMemoryCache memoryCache, IOptions<EmailSettings> emailSettings, IOptions<FirebaseSettings> firebaseSettings)
        {
            _context = context;
            _memoryCache = memoryCache;
            _emailSettings = emailSettings.Value;
            _firebaseSettings = firebaseSettings.Value;
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
        [HttpGet("requests")]
        public IActionResult Requests()
        {
            var key = Request.Headers["X-GCTGS-Key"];
            if (!_context.Users.Any(user => user.Key == key))
            {
                return StatusCode(403);
            }

            var currentUser = _context.Users.Single(user => user.Key == key);

            var requests = _context.Requests
                .Include(request => request.BoardGame)
                .Where(request => request.BoardGame.OwnerId == currentUser.Id)
                .Include(request => request.BoardGame.Owner)
                .Include(request => request.Requester)
                .OrderByDescending(request => request.DateTime).Take(10).ToList();

            return new ObjectResult(requests.Select(async request =>
                {
                    request.BoardGame.BggDetails = await request.BoardGame.GetBggDetails(_memoryCache);
                    return request;
                })
                .Select(request => request.Result));
        }

        [HttpPost("request/{boardGameId}")]
        public async Task<IActionResult> RequestBoardGame(int boardGameId)
        {
            var key = Request.Headers["X-GCTGS-Key"];
            if (!_context.Users.Any(user => user.Key == key))
            {
                return StatusCode(403);
            }

            var requester = _context.Users.Single(user => user.Key == key);
            var requested = _context.BoardGames
                .Include(boardGame => boardGame.Owner)
                .Single(boardGame => boardGame.Id == boardGameId);

            var request = new Request
            {
                DateTime = DateTime.Now,
                Requester = requester,
                BoardGame = requested
            };

            var emailClient = new MailgunClient(_emailSettings);
            await emailClient.SendNotificationEmail(requested.Owner.Email, requested.Owner.Name, requester.Name, requested.Name);

            if (requested.Owner.FcmToken != null)
            {
                var firebaseClient = new FirebaseClient(_firebaseSettings);
                await firebaseClient.SendRequestNotification(requested.Owner.FcmToken, requester, requested);
            }

            _context.Requests.Add(request);
            _context.SaveChanges();
            return StatusCode(200);
        }

        [HttpPut("token")]
        public IActionResult SetToken([FromBody] string token)
        {
            var key = Request.Headers["X-GCTGS-Key"];
            if (!_context.Users.Any(u => u.Key == key))
            {
                return StatusCode(403);
            }
            var user = _context.Users.Single(u => u.Key == key);
            user.FcmToken = token;
            _context.SaveChanges();
            return StatusCode(200);
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
