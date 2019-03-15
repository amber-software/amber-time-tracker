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
    public class CreateModel : TaskModelBase
    {
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager,
                          ISprintsService sprintsService,
                          IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        public  IActionResult OnGet()
        {
            return PopulateDropdownsAndShowPage();
        }

        [BindProperty]
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return PopulateDropdownsAndShowPage(Issue);
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
                    var sqlEx = ex?.InnerException as PostgresException;
                    if (sqlEx != null)
                        switch (sqlEx.SqlState)
                        {
                           case "23505": //SqlServer Violation Of Unique Index
                                var error = $"Cannot create Task with duplicated number '{Issue.TaskNumber}'";
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