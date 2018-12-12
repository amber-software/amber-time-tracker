using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimeTracking.Models;

namespace TimeTracking.Pages.Tasks
{
    public class CreateModel : TaskModelBase
    {
        public CreateModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {            
        }

        public  IActionResult OnGet()
        {
            PopulateSprintsDropDownList();
            return Page();
        }

        [BindProperty]
        public Issue Issue { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSprintsDropDownList();
                return Page();
            }

            var emptyTask = new Issue();

            if (await TryUpdateModelAsync<Issue>(
                 emptyTask,
                 "Issue",   // Prefix for form value.
                 s => s.SprintID, s => s.TaskNumber, s => s.TaskDescription))
            {
                context.Issue.Add(emptyTask);
                await context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            PopulateSprintsDropDownList(emptyTask.SprintID);
            return Page();
        }
    }
}