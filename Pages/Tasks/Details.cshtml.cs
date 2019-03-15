using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;
using TimeTracking.Services.Issues;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Tasks
{
    public class DetailsModel : TaskModelBase
    {
        public DetailsModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService,
                          IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Issue = await issueService.GetIssueWithSprint(id);

            if (Issue == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
