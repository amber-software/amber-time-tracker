using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimeTracking.Authorization;
using TimeTracking.Models;

namespace TimeTracking.Pages.Sprints
{
    public class CreateModel : PageModelBase
    {        
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, new Sprint(),
                                                      SprintsOperations.EditSprints);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        [BindProperty]
        public Sprint Sprint { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, Sprint,
                                                      SprintsOperations.EditSprints);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            context.Sprint.Add(Sprint);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}