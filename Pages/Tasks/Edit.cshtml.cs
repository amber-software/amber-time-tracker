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
using TimeTracking.Models;

namespace TimeTracking.Pages.Tasks
{
    public class EditModel : TaskModelBase
    {
        public EditModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
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

            Issue = await context.Issue.FirstOrDefaultAsync(m => m.ID == id);

            if (Issue == null)
            {
                return NotFound();
            }

            PopulateSprintsDropDownList(Issue.SprintID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                PopulateSprintsDropDownList(Issue.SprintID);
                return Page();
            }

            context.Attach(Issue).State = EntityState.Modified;

            var trackToUpdate = await context.Issue.FindAsync(id);

            if (await TryUpdateModelAsync<Issue>(
                 trackToUpdate,
                 "Issue",   // Prefix for form value.
                 s => s.SprintID, s => s.TaskNumber, s => s.TaskDescription))
            {
                await context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            PopulateSprintsDropDownList(Issue.SprintID);
            return Page();
        }
    }
}
