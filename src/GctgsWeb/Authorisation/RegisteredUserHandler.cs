using System.Linq;
using System.Threading.Tasks;
using GctgsWeb.Services;
using Microsoft.AspNetCore.Authorization;

namespace GctgsWeb.Authorisation
{
    public class RegisteredUserHandler : AuthorizationHandler<RegisteredUserRequirement>
    {
        private readonly GctgsContext _context;

        public RegisteredUserHandler(GctgsContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext authorizationContext, RegisteredUserRequirement requirement)
        {
            var crsid = authorizationContext.User.Identity.Name;
            if (crsid != null && _context.Users.Any(user => user.Crsid == crsid))
            {
                authorizationContext.Succeed(requirement);
            }
            return Task.FromResult(0);
        }
    }
}
