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
using System.ComponentModel;
using TimeTracking.Services.Sprints;
using TimeTracking.Services.Issues;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Pages.Tasks
{    
    public class TaskModelBase : PageModelBase
    {        
        public SelectList SprintNumberSL { get; set; }

        public SelectList TaskPrioritySL { get; set; }

        public SelectList StatusesSL { get; set; }

        public SelectList PlatformsSL { get; set; }

        public TaskModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager,
                                  ISprintsService sprintsService,
                                  IIssueService issueService) 
                                    : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        public PageResult PopulateDropdownsAndShowPage(Issue issue = null)
        {
            PopulateTaskPriorityDropDownList(issue?.Priority);
            PopulateSprintsDropDownList(issue?.SprintID);
            PopulateTaskStatusDropdownList(issue?.Status);
            PopulatePlatforms(issue?.Platform);

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

        private void PopulateTaskStatusDropdownList(object selectedStatus = null)
        {
            var statusType = typeof(Status);
            var values = Enum.GetValues(statusType);

            var statuses = new List<object>();
            foreach (var value in values)
            {
                var memInfo = statusType.GetMember(statusType.GetEnumName(value));
                 var descriptionAttribute = memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute;

                if (descriptionAttribute != null)
                {
                    statuses.Add(new { ID = (int)value, StatusName = descriptionAttribute.Description });
                }
            }

            StatusesSL = new SelectList(statuses,
                        "ID", "StatusName", selectedStatus);
        }

        private void PopulatePlatforms(Platform? selectedPlatform = null)
        {
            var platformType = typeof(Platform);
            var values = Enum.GetValues(platformType);

            var platforms = new List<object>();
            foreach (var value in values)
            {
                var memInfo = platformType.GetMember(platformType.GetEnumName(value));
                 var displayAttribute = memInfo[0]
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .FirstOrDefault() as DisplayAttribute;

                if (displayAttribute != null)
                {
                    platforms.Add(new { ID = (int)value, PlatformName = displayAttribute.Name });
                }
            }

            PlatformsSL = new SelectList(platforms,
                        "ID", "PlatformName", (int?)selectedPlatform);
        }

    }
}