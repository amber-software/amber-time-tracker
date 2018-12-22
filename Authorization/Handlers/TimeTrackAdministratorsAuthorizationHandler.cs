using System.Threading.Tasks;
using TimeTracking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace TimeTracking.Authorization
{
    public class TimeTrackAdministratorsAuthorizationHandler
                    : AdministratorsAuthorizationHandler<TimeTrack>
    {        
        public TimeTrackAdministratorsAuthorizationHandler(UserManager<IdentityUser> userManager)
            : base(userManager)
        {            
        }        
    }
}