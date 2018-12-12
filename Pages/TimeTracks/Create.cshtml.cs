using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{
    public class CreateModel : TimeTrackModelBase
    {        
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        public IActionResult OnGet()
        {            
            PopulateIssuesDropDownList();
            return Page();
        }

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateIssuesDropDownList();
                return Page();
            }

            var emptyTrack = new TimeTrack();
            emptyTrack.OwnerID = userManager.GetUserId(User);

            if (await TryUpdateModelAsync<TimeTrack>(
                 emptyTrack,
                 "TimeTrack",   // Prefix for form value.
                 s => s.IssueID, s => s.SpentHours, s => s.TrackingDate))
            {
                context.TimeTrack.Add(emptyTrack);
                await context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            PopulateIssuesDropDownList(emptyTrack.IssueID);
            return Page();
        }
    }
}