using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{
    public class EditModel : TimeTrackModelBase
    {        
        public EditModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeTrack = await context.TimeTrack.FirstOrDefaultAsync(m => m.ID == id);

            if (TimeTrack == null)
            {
                return NotFound();
            }

            PopulateIssuesDropDownList(TimeTrack.IssueID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                PopulateIssuesDropDownList(TimeTrack.IssueID);
                return Page();
            }

            context.Attach(TimeTrack).State = EntityState.Modified;

            var trackToUpdate = await context.TimeTrack.FindAsync(id);

            if (await TryUpdateModelAsync<TimeTrack>(
                 trackToUpdate,
                 "TimeTrack",   // Prefix for form value.
                   s => s.IssueID, s => s.SpentHours, s => s.TrackingDate))
            {
                await context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            PopulateIssuesDropDownList(TimeTrack.IssueID);
            return Page();
        }        
    }
}
