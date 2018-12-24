using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Pages;
using TimeTracking.Models;
using Microsoft.AspNetCore.Authorization;

namespace TimeTracking.Pages.Tasks
{    
    public class TaskModelBase : PageModelBase
    {        
        public SelectList SprintNumberSL { get; set; }

        public SelectList TaskPrioritySL { get; set; }

        public TaskModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {            
        }

        public PageResult PopulateDropdownsAndShowAgain(Issue issue = null)
        {
            PopulateTaskPriorityDropDownList(issue?.Priority);
            PopulateSprintsDropDownList(issue?.SprintID);

            return Page();
        }

        private void PopulateSprintsDropDownList(object selectedSprint = null)
        {
            var sprintsQuery = from d in context.Sprint
                                   orderby d.SprintNumber // Sort by name.
                                   select d;

            SprintNumberSL = new SelectList(sprintsQuery.AsNoTracking(),
                        "ID", "SprintNumber", selectedSprint);
        }

        private void PopulateTaskPriorityDropDownList(object selectedPriority = null)
        {
            var values = Enum.GetValues(typeof(TaskPriority));

            var priorities = new List<object>();
            foreach (var value in values)
            {
                priorities.Add(new { ID = (int)value, PriorityName = value });
            }

            TaskPrioritySL = new SelectList(priorities,
                        "ID", "PriorityName", selectedPriority);
        }
    }
}