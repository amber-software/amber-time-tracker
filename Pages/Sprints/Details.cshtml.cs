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
    public class DetailsModel : PageModelBase
    {        
        public DetailsModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {            
        }

        public Sprint Sprint { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sprint = await context.Sprint.FirstOrDefaultAsync(m => m.ID == id);

            if (Sprint == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
