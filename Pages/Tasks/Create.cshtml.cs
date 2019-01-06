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
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Tasks
{
    public class CreateModel : TaskModelBase
    {
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
        {            
        }

        public  IActionResult OnGet()
        {
            return PopulateDropdownsAndShowAgain();
        }

        [BindProperty]
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return PopulateDropdownsAndShowAgain(Issue);
            }

            var emptyTask = new Issue();

            if (await TryUpdateModelAsync<Issue>(
                 emptyTask,
                 "Issue",   // Prefix for form value.
                 s => s.SprintID, s => s.TaskNumber, s => s.TaskDescription, s => s.Priority, s => s.Estimate, s => s.Remaining))
            {
                context.Issue.Add(emptyTask);

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
                                var error = $"Cannot create Task with duplicated number '{Issue.TaskNumber}'";
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