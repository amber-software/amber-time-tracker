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
using TimeTracking.Authorization;
using TimeTracking.Models;
using TimeTracking.Services.Issues;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.TimeTracks
{
    public class EditModel : TimeTrackModelBase
    {        
        public EditModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService,
                           IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        private IQueryable<TimeTrack> timeTrackQuery => from tt in context.TimeTrack.Include(t => t.Issue).ThenInclude(i => i.Sprint)
                                                        select tt;

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnGetAsync(int timeTrackId)
        {
            // Validation                                                
            TimeTrack = await timeTrackQuery
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.ID == timeTrackId);
        
            if (TimeTrack == null)
            {
                throw new ApplicationException($"There is no suitable time track with ID={timeTrackId} in database!");
            }

            if (userManager.GetUserId(User) != TimeTrack.OwnerID)
            {
                TargetUserId = TimeTrack.OwnerID;
                if (! await AllowedToEditTimeTracksOfAnother())

                return new ChallengeResult();
            }


            // Set data for editing
            return await PopulateDropdownsAndShowPage(TimeTrack.OwnerID, TimeTrack.Issue.Sprint?.ID, TimeTrack.IssueID);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validation
            string targetUserId = null;

            if (userManager.GetUserId(User) != TimeTrack.OwnerID)
            {
                targetUserId = TimeTrack.OwnerID;
                if (! await AllowedToEditTimeTracksOfAnother())
                return new ChallengeResult();
            }

            var trackToUpdate = await timeTrackQuery                                        
                                        .FirstOrDefaultAsync(t => t.ID == TimeTrack.ID);

            if (trackToUpdate == null)
            {
                throw new ApplicationException($"There is no suitable time track with ID={TimeTrack.ID} in database!");
            }
            
            if (!ModelState.IsValid)
            {                
                return await PopulateDropdownsAndShowPage(trackToUpdate.OwnerID, trackToUpdate.Issue.Sprint?.ID, trackToUpdate.IssueID);                
            }
            
            if (TimeTrack.TrackingDate != trackToUpdate.TrackingDate)
            {
                var trackForIssueInAnotherDate = await timeTrackQuery                                        
                                        .FirstOrDefaultAsync(t => t.OwnerID == TimeTrack.OwnerID &&
                                                                  t.IssueID == TimeTrack.IssueID && 
                                                                  t.TrackingDate == TimeTrack.TrackingDate);

                if (trackForIssueInAnotherDate != null)
                {
                    trackForIssueInAnotherDate.SpentHours += TimeTrack.SpentHours;
                    
                    context.TimeTrack.Remove(trackToUpdate);
                    
                    await context.SaveChangesAsync();
                    return RedirectToPage("./Index", null, new { id = targetUserId });
                    
                }        
            }

            if (TimeTrack.SpentHours <= 0)
                context.TimeTrack.Remove(trackToUpdate);
            else if (!await TryUpdateModelAsync<TimeTrack>(
                 trackToUpdate,
                 "TimeTrack",   // Prefix for form value.
                   s => s.IssueID, s => s.SpentHours, s => s.TrackingDate, s => s.Description))
            {
                return await PopulateDropdownsAndShowPage(trackToUpdate.OwnerID, trackToUpdate.Issue.Sprint?.ID, trackToUpdate.IssueID);                                
            }
                        
            await context.SaveChangesAsync();                                
            return RedirectToPage($"./Index", null, new { id = targetUserId } );            
        }        
    }
}
