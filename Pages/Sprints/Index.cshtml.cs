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
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Sprints
{
    public class IndexModel : PageModelBase
    {        
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {            
        }

        public IList<Sprint> Sprint { get;set; }

        public async Task OnGetAsync()
        {
            Sprint = await sprintsService.GetAllSprints();
        }
    }
}
