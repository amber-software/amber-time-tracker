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

namespace TimeTracking.Pages.Tasks
{
    public class DeleteModel : TaskModelBase
    {
        public DeleteModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {            
        }

        [BindProperty]
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Issue = await context.Issue.Include(c => c.Sprint).FirstOrDefaultAsync(m => m.ID == id);

            if (Issue == null)
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

            Issue = await context.Issue.FindAsync(id);

            if (Issue != null)
            {
                context.Issue.Remove(Issue);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
