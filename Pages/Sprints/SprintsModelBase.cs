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
using TimeTracking.Services.Issues;

namespace TimeTracking.Pages.Sprints
{    
    public class SprintsModelBase : PageModelBase
    {                
        public SprintsModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager,
                                  ISprintsService sprintsService,
                                  IIssueService issueService) 
                                    : base(context, authorizationService, userManager, sprintsService, issueService)
        {
        }

        public async Task<bool> CheckSprintDates(Sprint sprint)
        {
            if (sprint.StartDate > sprint.StopDate)
            {
                ModelState.AddModelError("Sprint.StartDate", $"Sprint start date can't be larger than stop date");

                return false;
            }

            var overlappedSprint = (await sprintsService.GetAllSprints())
                                    .FirstOrDefault(s => s.ID != sprint.ID && 
                                                         s.StartDate <= sprint.StopDate && 
                                                         sprint.StartDate <= s.StopDate);
            if (overlappedSprint != null)
            {
                ModelState.AddModelError("Sprint.StartDate", $"Sprint {overlappedSprint.SprintNumber} already existed for these date");
                ModelState.AddModelError("Sprint.StopDate", $"Sprint {overlappedSprint.SprintNumber} already existed for these date");
                
                return false;
            }

            return true;
        }

    }
}