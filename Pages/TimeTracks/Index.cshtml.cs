using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Pages;
using TimeTracking.Models;
using Microsoft.AspNetCore.Authorization;
using TimeTracking.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TimeTracking.Pages.TimeTracks
{
    public class IndexModel : TimeTrackModelBase
    {
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        public IList<IssueTrack> TimeTracks { get;set; }

        public IList<DateTime> SprintDays { get; set; }
                
        public async Task<IActionResult> OnGetAsync(string id, int? sprintId)
        {
            if (!string.IsNullOrEmpty(id) &&
                !(await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      TimeTracksOperations.ViewStatistics))
                .Succeeded)
            {
                return new ChallengeResult();
            }

            var targetUserId = string.IsNullOrEmpty(id) ? userManager.GetUserId(User) : id;

            var sprint = await GetTargetSprintOrCurrentSprint(sprintId);

            SprintDays = Enumerable.Range(0, 1 + sprint.StopDate
                                                        .Subtract(sprint.StartDate).Days)
                                                        .Select(offset => sprint.StartDate.AddDays(offset))
                                                        .ToList();

            TimeTracks = sprint.Issues
                            .Where(i => i.TimeTracks.Any(t => t.OwnerID == targetUserId))
                            .Select(i => new IssueTrack()
                            {
                                IssueNumber = i.TaskNumber,
                                IssueDescription = i.TaskDescription,
                                Estimate = 0,
                                RemainingTime = 0,
                                LoggedTimes = GetIssueSpentTimesByDays(targetUserId, i, SprintDays)
                            })                            
                            .ToList();            

            TargetUserId = id;
            PopulateSprintsDropDownList(sprint.ID);
            return Page();
        }

        private IEnumerable<TimeTrackLogTime> GetIssueSpentTimesByDays(string targetUserId, Issue issue, IEnumerable<DateTime> days)
        {
            var spentTimes = new List<TimeTrackLogTime>();
            foreach (var date in days)
            {
                var timeTrack = issue.TimeTracks.FirstOrDefault(t => t.OwnerID == targetUserId && t.TrackingDate == date);
                
                spentTimes.Add(new TimeTrackLogTime()
                                    {
                                        TimeTrackID = timeTrack?.ID,
                                        Hours = timeTrack?.SpentHours 
                                    });
            }

            return spentTimes;
        }        
    }
}
