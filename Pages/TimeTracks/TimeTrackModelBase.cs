using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Pages;
using TimeTracking.Models;
using TimeTracking.Authorization;
using Microsoft.AspNetCore.Authorization;
using TimeTracking.Services.Sprints;
using System.ComponentModel;
using TimeTracking.Services.Issues;

namespace TimeTracking.Pages.TimeTracks
{    
    public class TimeTrackModelBase : PageModelBase
    {                
        public int? TargetSprintId { get; set; }

        public int? TargetPlatformId { get; set; }

        public string TargetUserId { get; set; }

        public SelectList IssueNameSL { get; set; }      

        public SelectList SprintsSL { get; set; }

        public SelectList PlatformsSL { get; set; }
        
        public TimeTrackModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager,
                                  ISprintsService sprintsService,
                                  IIssueService issueService) 
                                    : base(context, authorizationService, userManager, sprintsService, issueService)
        {
        }

        protected async Task<bool> AllowedToEditTimeTracksOfAnother()
        {
            return (await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.EditTimeTracksOfOthers)).Succeeded;
        }        

        private void PopulateIssuesDropDownList(IEnumerable<Sprint> allSprints, int? selectedSprintId, int? selectedIssueId = null)
        {
            var selectedSprint = allSprints.FirstOrDefault(s => s.ID == selectedSprintId);
            var issues = selectedSprint?.Issues ?? 
                                context.Issue.AsNoTracking().ToArray();
            IssueNameSL = new SelectList(issues.OrderBy(i => i.TaskNumber),
                        "ID", "TaskNumber", selectedIssueId);
        }

        private void PopulateSprintsDropDownList(IEnumerable<Sprint> allSprints, int? selectedSprintId)
        {
            var selectedSprint = allSprints.FirstOrDefault(s => s.ID == selectedSprintId);
            SprintsSL = new SelectList(allSprints, "ID", "SprintNumber", selectedSprintId);
        }
        
        public async Task<PageResult> PopulateDropdownsAndShowPage(string userId, int? sprintId, int? selectedIssueId = null)
        {
            TargetSprintId = sprintId;
            TargetUserId = userId;
            
            var allSprints = await sprintsService.GetAllSprints();            
            PopulateSprintsDropDownList(allSprints, sprintId);
            PopulateIssuesDropDownList(allSprints, sprintId, selectedIssueId);

            return Page();
        }
    }
}