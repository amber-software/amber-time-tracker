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

namespace TimeTracking.Pages.TimeTracks
{    
    public class TimeTrackModelBase : PageModelBase
    {                
        public int? TargetSprintId { get; set; }

        public string TargetUserId { get; set; }   

        public SelectList IssueNameSL { get; set; }      

        public SelectList SprintsSL { get; set; }

        public TimeTrackModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager,
                                  ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {
        }

        protected async Task<bool> AllowedToEditTimeTracksOfAnother()
        {
            return (await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.EditTimeTracksOfOthers)).Succeeded;
        }        

        protected void PopulateIssuesDropDownList(Sprint sprint, object selectedIssue = null)
        {            
            IssueNameSL = new SelectList(sprint.Issues.OrderBy(i => i.TaskNumber),
                        "ID", "TaskNumber", selectedIssue);
        }

        protected async Task PopulateSprintsDropDownList(object selectedSprint = null)
        {            
            SprintsSL = new SelectList(await sprintsService.GetAllSprints(),
                        "ID", "SprintNumber", selectedSprint);
        }                

        
    }
}