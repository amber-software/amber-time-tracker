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
    public class CreateModel : TimeTrackModelBase
    {        
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService,
                           IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }        
        
        public async Task<IActionResult> OnGet(string id, int? sprintId)
        {
            // Validation
            if (!string.IsNullOrEmpty(id) &&
                userManager.GetUserId(User) != id &&
                ! await AllowedToEditTimeTracksOfAnother())
                return new ChallengeResult();

            var sprint = await sprintsService.GetTargetSprint(sprintId);

            // Set data for creation
            return await PopulateDropdownsAndShowPage(id, sprintId);            
        }        

        public async Task<IActionResult> OnPostAsync(string id, int? sprintId)
        {
            // Validation
            if (!string.IsNullOrEmpty(id) &&
                userManager.GetUserId(User) != id &&
                ! await AllowedToEditTimeTracksOfAnother())
                return new ChallengeResult();

            var sprint = await sprintsService.GetTargetSprint(sprintId);           

            if (!ModelState.IsValid)
            {
                return await PopulateDropdownsAndShowPage(id, sprintId, TimeTrack.IssueID, TimeTrack.Platform);
            }

            Issue targetIssue = await issueService.GetTargetIssue(TimeTrack.IssueID);
            
            if (targetIssue == null)
                throw new ApplicationException($"There is no suitable issue ID={TimeTrack.IssueID} in database!");

            var targetUserId = string.IsNullOrEmpty(id) ? userManager.GetUserId(User) : id;
            if (targetIssue.TimeTracks.Any(t => t.OwnerID == targetUserId && t.TrackingDate == TimeTrack.TrackingDate))
            {
                ModelState.AddModelError("TimeTrack.TrackingDate", $"Time for task '{targetIssue.TaskNumber}' is already set for date '{TimeTrack.TrackingDate.ToShortDateString()}'");

                return await PopulateDropdownsAndShowPage(id, sprintId, TimeTrack.IssueID, TimeTrack.Platform);
            }

            // Create new track
            var emptyTrack = new TimeTrack();
            emptyTrack.OwnerID = targetUserId;

            if (await TryUpdateModelAsync<TimeTrack>(
                 emptyTrack,
                 "TimeTrack",   // Prefix for form value.
                 s => s.IssueID, s => s.SpentHours, s => s.TrackingDate, s => s.Platform, s => s.Description))
            {
                context.TimeTrack.Add(emptyTrack);
                await context.SaveChangesAsync();

                return RedirectToPage("./Index", null, new { id = id, sprintId = sprintId });
            }

            ModelState.AddModelError(string.Empty, $"Can't set time for task '{targetIssue.TaskNumber}' for date '{TimeTrack.TrackingDate}'");

            return await PopulateDropdownsAndShowPage(id, sprintId, TimeTrack.IssueID, TimeTrack.Platform);
        }        
    }
}