using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Authorization;
using TimeTracking.Models;

namespace TimeTracking.Pages
{    
    [AllowAnonymous]
    public class IndexModel : PageModelBase
    {        
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        public IList<TimeTrackingVM> TimeTracking { get;set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.ViewStatistics);
            if (!isAuthorized.Succeeded)
            {
                return RedirectToPage("/TimeTracks/Index");
            }

            var nowDate = DateTime.Now.Date;
                 
            var sprint = await context.Sprint
                                        .FirstOrDefaultAsync(s => s.StartDate >= nowDate && nowDate < s.StopDate);            

            TimeTracking = await context.Users
                            .AsNoTracking()
                            .Select(u => new TimeTrackingVM
                            {
                                UserID = u.Id,
                                UserName = u.UserName,
                                TotalSpentHours = context.TimeTrack.Where(t => t.OwnerID == u.Id).Sum(t => t.SpentHours),
                                // TotalRemainingHours == 0
                            })                                                                                    
                            .ToListAsync();
        

            return Page();
        }
    }
}
