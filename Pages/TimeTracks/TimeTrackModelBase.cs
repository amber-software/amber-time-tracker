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
using TimeTracking.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace TimeTracking.Pages.TimeTracks
{    
    public class TimeTrackModelBase : PageModelBase
    {                
        public int? TargetSprintId { get; set; }

        public string TargetUserId { get; set; }   

        public SelectList IssueNameSL { get; set; }      

        public SelectList SprintsSL { get; set; }  

        public TimeTrackModelBase(TimeTracking.Models.TimeTrackDataContext context, 
                                  IAuthorizationService authorizationService,
                                  UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {
        }

        protected async Task<bool> AllowedToEditTimeTracksOfAnother()
        {
            return (await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.EditTimeTracksOfOthers)).Succeeded;
        }        

        protected void PopulateIssuesDropDownList(Sprint sprint, object selectedIssue = null)
        {            
            IssueNameSL = new SelectList(sprint.Issues.OrderBy(i => i.TaskNumber),
                        "ID", "TaskNumber", selectedIssue);
        }

        protected void PopulateSprintsDropDownList(object selectedSprint = null)
        {            
            SprintsSL = new SelectList(sprintsQuery.AsNoTracking(),
                        "ID", "SprintNumber", selectedSprint);
        }        

        protected IQueryable<Sprint> sprintsQuery => from sp in context.Sprint.Include(s => s.Issues).ThenInclude(i => i.TimeTracks)
                                                    orderby sp.StartDate descending // Sort by name.
                                                    select sp;

        protected async Task<Sprint> GetTargetSprintOrCurrentSprint(int? sprintId)
        {
            var nowDate = DateTime.Now.Date;            

            var sprint = sprintId.HasValue ?
                            await sprintsQuery.AsNoTracking().FirstOrDefaultAsync(s => s.ID == sprintId) :
                            await sprintsQuery.AsNoTracking().FirstOrDefaultAsync(s => s.StartDate <= nowDate && nowDate < s.StopDate);

            if (sprint == null)
                throw new ApplicationException("There is no suitable sprint in database!");

            return sprint;
        }
    }
}