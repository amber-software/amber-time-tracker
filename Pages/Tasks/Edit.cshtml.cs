using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            return PopulateDropdownsAndShowAgain(Issue);
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {                
                return PopulateDropdownsAndShowAgain(Issue);
            }

            context.Attach(Issue).State = EntityState.Modified;

            var trackToUpdate = await context.Issue.FindAsync(id);

            if (await TryUpdateModelAsync<Issue>(
                 trackToUpdate,
                 "Issue",   // Prefix for form value.
                 s => s.SprintID, s => s.TaskNumber, s => s.TaskDescription, s => s.Priority, s => s.Estimate, s => s.Remaining))
            {
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    var sqlEx = ex?.InnerException as SqlException;
                    if (sqlEx != null)
                        switch (sqlEx.Number)
                        {
                           case 2601: //SqlServer Violation Of Unique Index
                                var error = $"Cannot modify Task with existed number '{Issue.TaskNumber}'";
                                ModelState.AddModelError("Issue.TaskNumber", error);

                                return PopulateDropdownsAndShowAgain(Issue);
                           default:
                              throw;
                        }
                }

                return RedirectToPage("./Index");
            }

            return PopulateDropdownsAndShowAgain(Issue);
        }
    }
}
