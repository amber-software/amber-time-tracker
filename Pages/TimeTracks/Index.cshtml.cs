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
using TimeTracking.Services.Sprints;
using System.ComponentModel.DataAnnotations;
using TimeTracking.Services.Issues;

namespace TimeTracking.Pages.TimeTracks
{
    public class IndexModel : TimeTrackModelBase
    {
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService,
                           IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        public IList<IssueTrack> SpentTimes { get;set; }

        public IList<DateTime> SelectedDays { get; set; }

        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To")]
        public DateTime? StopDate { get; set; }
                
        public async Task<IActionResult> OnGetAsync(string id, int? sprintId, DateTime? startDate, DateTime? stopDate)
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

            var sprint = await sprintsService.GetTargetSprint(sprintId);
            if (startDate.HasValue && stopDate.HasValue)
            {
                StartDate = startDate;
                StopDate = stopDate;
            }
            else if (sprint != null)
            {
                StartDate = sprint.StartDate;
                StopDate = sprint.StopDate;
            }
            else
            {
                var thisWeekStart = DateTime.Now.Date;
                while (thisWeekStart.DayOfWeek != DayOfWeek.Monday)
                    thisWeekStart = thisWeekStart.AddDays(-1);

                StartDate = thisWeekStart;
                StopDate = thisWeekStart.AddDays(6);
            }

            SelectedDays = Enumerable.Range(0, 1 + StopDate.Value
                                                        .Subtract(StartDate.Value).Days)
                                                        .Select(offset => StartDate.Value.AddDays(offset))
                                                        .ToList();

            var issues = sprint?.Issues ?? await issueService.GetAllIssues();
            SpentTimes = issues
                            .Where(i => i.TimeTracks.Any(t => t.OwnerID == targetUserId && 
                                                              t.TrackingDate >= StartDate.Value && 
                                                              t.TrackingDate <= StopDate.Value))
                            .Select(i => new IssueTrack()
                            {
                                IssueNumber = i.TaskNumber,
                                IssueDescription = i.TaskDescription,
                                Estimate = i.Estimate,
                                RemainingTime = i.Remaining,
                                LoggedTimes = GetIssueSpentTimesByDays(targetUserId, i, SelectedDays)
                            })                            
                            .ToList();            
            
            return await PopulateDropdownsAndShowPage(id, sprintId);
        }

        private IList<TimeTrackLogTime> GetIssueSpentTimesByDays(string targetUserId, Issue issue, IEnumerable<DateTime> days)
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
