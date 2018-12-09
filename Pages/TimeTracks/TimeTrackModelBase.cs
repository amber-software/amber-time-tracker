using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{    
    public class TimeTrackModelBase : PageModel
    {
        protected readonly TimeTracking.Models.TimeTrackDataContext context;
        protected readonly UserManager<IdentityUser> userManager;

        public SelectList IssueNameSL { get; set; }

        public TimeTrackModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  UserManager<IdentityUser> userManager)
                                  : base()
        {
            this.context = context;
            this.userManager = userManager;
        }

        public void PopulateIssuesDropDownList(object selectedIssue = null)
        {
            var issuesQuery = from d in context.Issue
                                   orderby d.TaskNumber // Sort by name.
                                   select d;

            IssueNameSL = new SelectList(issuesQuery.AsNoTracking(),
                        "ID", "TaskNumber", selectedIssue);
        }
    }
}