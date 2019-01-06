using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Authorization;
using TimeTracking.Models;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Sprints
{
    public class DeleteModel : PageModelBase
    {        
        public DeleteModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {            
        }

        [BindProperty]
        public Sprint Sprint { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, new Sprint(),
                                                      SprintsOperations.EditSprints);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Sprint = await context.Sprint.FirstOrDefaultAsync(m => m.ID == id);

            if (Sprint == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, Sprint,
                                                      SprintsOperations.EditSprints);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Sprint = await context.Sprint.FindAsync(id);

            if (Sprint != null)
            {
                context.Sprint.Remove(Sprint);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
