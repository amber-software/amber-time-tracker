using System.Threading.Tasks;
using TimeTracking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace TimeTracking.Authorization
{
    public class SprintsAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Sprint>
    {
        private UserManager<IdentityUser> _userManager;

        public SprintsAdministratorsAuthorizationHandler(UserManager<IdentityUser> 
            userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement, 
                                    Sprint resource)
        {
            if (context.User == null)
            {
                return;
            }

            // Administrators can do anything.
            var userIdentity = await _userManager.GetUserAsync(context.User);
            
            if (await _userManager.IsInRoleAsync(userIdentity, Constants.AdministratorsRole))
            {
                context.Succeed(requirement);
            }        
        }
    }
}