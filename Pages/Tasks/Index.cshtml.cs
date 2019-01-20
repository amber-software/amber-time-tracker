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
using TimeTracking.Services.Sprints;
using TimeTracking.Services.Issues;

namespace TimeTracking.Pages.Tasks
{
    public class IndexModel : TaskModelBase
    {
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService,
                          IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        public IList<Issue> Issue { get;set; }

        public async Task OnGetAsync()
        {
            Issue = await issueService.GetAllIssuesWithSprints();                            
        }
    }
}
