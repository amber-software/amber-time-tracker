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
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.TimeTracks
{
    public class CreateModel : TimeTrackModelBase
    {        
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService) 
                           : base(context, authorizationService, userManager, sprintsService)
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

            var sprint = await sprintsService.GetTargetSprintOrCurrentSprint(sprintId);

            // Set data for creation
            PopulateCreateTimeTrackIdentifiers(id, sprintId);            
            PopulateIssuesDropDownList(sprint);

            return Page();
        }        

        public async Task<IActionResult> OnPostAsync(string id, int? sprintId)
        {
            // Validation
            if (!string.IsNullOrEmpty(id) &&
                userManager.GetUserId(User) != id &&
                ! await AllowedToEditTimeTracksOfAnother())
                return new ChallengeResult();

            var sprint = await sprintsService.GetTargetSprintOrCurrentSprint(sprintId);           

            if (!ModelState.IsValid)
            {
                PopulateCreateTimeTrackIdentifiers(id, sprintId);            
                PopulateIssuesDropDownList(sprint);

                return Page();
            }

            var targetIssue = sprint.Issues.FirstOrDefault(i => i.ID == TimeTrack.IssueID);
            if (targetIssue == null)
                throw new ApplicationException($"There is no suitable issue ID={TimeTrack.IssueID} in database!");

            var targetUserId = string.IsNullOrEmpty(id) ? userManager.GetUserId(User) : id;
            if (targetIssue.TimeTracks.Any(t => t.OwnerID == targetUserId && t.TrackingDate == TimeTrack.TrackingDate))
            {
                ModelState.AddModelError("TimeTrack.TrackingDate", $"Time for task '{targetIssue.TaskNumber}' is already set for date '{TimeTrack.TrackingDate.ToShortDateString()}'");

                PopulateCreateTimeTrackIdentifiers(id, sprintId);            
                PopulateIssuesDropDownList(sprint);

                return Page();
            }

            // Create new track
            var emptyTrack = new TimeTrack();
            emptyTrack.OwnerID = targetUserId;

            if (await TryUpdateModelAsync<TimeTrack>(
                 emptyTrack,
                 "TimeTrack",   // Prefix for form value.
                 s => s.IssueID, s => s.SpentHours, s => s.TrackingDate))
            {
                context.TimeTrack.Add(emptyTrack);
                await context.SaveChangesAsync();

                return RedirectToPage("./Index", null, new { id = id, sprintId = sprintId });
            }

            ModelState.AddModelError(string.Empty, $"Can't set time for task '{targetIssue.TaskNumber}' for date '{TimeTrack.TrackingDate}'");

            PopulateCreateTimeTrackIdentifiers(id, sprintId);            
            PopulateIssuesDropDownList(sprint, emptyTrack.IssueID);

            return Page();
        }

        private void PopulateCreateTimeTrackIdentifiers(string userId, int? sprintId)
        {
            TargetSprintId = sprintId;
            TargetUserId = userId;
        }
    }
}