using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Pages;
using TimeTracking.Models;
using Microsoft.AspNetCore.Authorization;
using TimeTracking.Authorization;

namespace TimeTracking.Pages.TimeTracks
{
    public class IndexModel : TimeTrackModelBase
    {        
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        public IList<TimeTrack> TimeTrack { get;set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id) &&
                !(await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.ViewStatistics))
                .Succeeded)
            {
                return new ChallengeResult();
            }

            var currentUserId = string.IsNullOrEmpty(id) ? userManager.GetUserId(User) : id;

            TimeTrack = await context.TimeTrack
                            .Where(t => t.OwnerID == currentUserId)
                            .Include(c => c.Issue)
                            .AsNoTracking()
                            .ToListAsync();

            return Page();
        }
    }
}
