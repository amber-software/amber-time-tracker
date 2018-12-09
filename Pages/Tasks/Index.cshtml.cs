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

namespace TimeTracking.Pages.Tasks
{
    public class IndexModel : PageModelBase
    {        
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context, 
                          UserManager<IdentityUser> userManager)
                                  : base(context, userManager)
        {            
        }

        public IList<Issue> Issue { get;set; }

        public async Task OnGetAsync()
        {
            Issue = await context.Issue.ToListAsync();
        }
    }
}
