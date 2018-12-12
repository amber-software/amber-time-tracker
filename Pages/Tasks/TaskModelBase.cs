using System;
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

        public TaskModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {            
        }

        public void PopulateSprintsDropDownList(object selectedSprint = null)
        {
            var sprintsQuery = from d in context.Sprint
                                   orderby d.SprintNumber // Sort by name.
                                   select d;

            SprintNumberSL = new SelectList(sprintsQuery.AsNoTracking(),
                        "ID", "SprintNumber", selectedSprint);
        }
    }
}