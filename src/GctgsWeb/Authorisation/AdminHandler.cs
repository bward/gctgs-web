using System.Linq;
using System.Threading.Tasks;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Authorization;

namespace GctgsWeb.Authorisation
{
    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly GctgsContext _context;

        public AdminHandler(GctgsContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext authorizationContext, AdminRequirement requirement)
        {
            if (_context.Users.Any(user => user.Crsid == authorizationContext.User.Identity.Name && user.Admin))
            {
                authorizationContext.Succeed(requirement);
            }
            return Task.FromResult(0);
        }
    }
}
