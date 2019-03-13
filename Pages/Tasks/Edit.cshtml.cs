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
using Npgsql;
using TimeTracking.Models;
using TimeTracking.Services.Issues;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Tasks
{
    public class EditModel : TaskModelBase
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
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Issue = await issueService.GetTargetIssue(id);

            if (Issue == null)
            {
                return NotFound();
            }

            return PopulateDropdownsAndShowPage(Issue);
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {                
                return PopulateDropdownsAndShowPage(Issue);
            }

            context.Attach(Issue).State = EntityState.Modified;

            var taskToUpdate = await issueService.GetTargetIssue(id);

            if (await TryUpdateModelAsync<Issue>(
                 taskToUpdate,
                 "Issue",   // Prefix for form value.
                 s => s.SprintID, s => s.TaskNumber, s => s.TaskDescription, s => s.Priority, s => s.Estimate, s => s.Remaining, s => s.Status))
            {
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    var sqlEx = ex?.InnerException as PostgresException;
                    if (sqlEx != null)
                        switch (sqlEx.SqlState)
                        {
                           case "23505": //SqlServer Violation Of Unique Index
                                var error = $"Cannot modify Task with existed number '{Issue.TaskNumber}'";
                                ModelState.AddModelError("Issue.TaskNumber", error);

                                return PopulateDropdownsAndShowPage(Issue);
                           default:
                              throw;
                        }
                }

                return RedirectToPage("./Index");
            }

            return PopulateDropdownsAndShowPage(Issue);
        }
    }
}
