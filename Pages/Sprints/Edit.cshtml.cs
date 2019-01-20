using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Authorization;
using TimeTracking.Models;
using TimeTracking.Services.Issues;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Sprints
{
    public class EditModel : SprintsModelBase
    {        
        public EditModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService,
                          IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!await CheckSprintDates(Sprint))
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

            context.Attach(Sprint).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SprintExists(Sprint.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SprintExists(int id)
        {
            return context.Sprint.Any(e => e.ID == id);
        }
    }
}
