using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Authorization;
using TimeTracking.Models;
using TimeTracking.Services.Issues;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Reports
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService,
                           IIssueService issueService) 
                           : base(context, authorizationService, userManager, sprintsService, issueService)
        {            
        }

        public IList<IssueTrack> SpentTimes { get; set; }

        public IList<DateTime> ReportDays { get; set; }

        public SelectList SprintsSL { get; set; }

        public int? TargetSprintId { get; set; }

        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To")]
        public DateTime? StopDate { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, DateTime? startDate, DateTime? stopDate)
        {
            if (!(await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      ReportOperations.ViewReport))
                .Succeeded)
            {
                return new ChallengeResult();
            }
            
            Sprint targetSprint = await sprintsService.GetTargetSprint(id);
            if (startDate.HasValue && stopDate.HasValue)
            {
                StartDate = startDate;
                StopDate = stopDate;
            }
            else if (targetSprint != null)
            {
                StartDate = targetSprint.StartDate;
                StopDate = targetSprint.StopDate;
            }
            else
            {
                var thisWeekStart = DateTime.Now.Date;
                while (thisWeekStart.DayOfWeek != DayOfWeek.Monday)
                    thisWeekStart = thisWeekStart.AddDays(-1);

                StartDate = thisWeekStart;
                StopDate = thisWeekStart.AddDays(6);
            }

            if (!StartDate.HasValue || !StopDate.HasValue)
                throw new ApplicationException("Can't create report: not enough parameters");            

            ReportDays = Enumerable.Range(0, 1 + StopDate.Value
                                                        .Subtract(StartDate.Value).Days)
                                                        .Select(offset => StartDate.Value.AddDays(offset))
                                                        .ToList();

            var issues = (id ?? 0) >= 0 ?
                            targetSprint?.Issues ?? await issueService.GetAllIssues() :
                            await issueService.GetIssuesWithoutSprints();
            SpentTimes = issues
                            .Where(i => i.TimeTracks.Any(t => t.TrackingDate >= StartDate.Value && t.TrackingDate < StopDate.Value))
                            .Select(i => new IssueTrack()
                            {
                                IssueNumber = i.TaskNumber,
                                IssueDescription = i.TaskDescription,
                                Estimate = i.Estimate,
                                RemainingTime = i.Remaining,
                                LoggedTimes = GetIssueSpentTimesByDays(i, ReportDays),
                                Status = i.Status,
                                Priority = i.Priority
                            })                            
                            .ToList();   
            
            TargetSprintId = id;
            await PopulateSprintsDropDownList(id);
            return Page();
        }

        private async Task PopulateSprintsDropDownList(object selectedSprintID = null)
        {
            var selectValues = (await sprintsService.GetAllSprints())
                                .Select(s => new { ID = s.ID, Text = s.SprintNumber })
                                .ToList();
            selectValues.Add(new { ID = -1, Text = "Without Sprint" });

            SprintsSL = new SelectList(selectValues, "ID", "Text", selectedSprintID);            
        }

        private IList<TimeTrackLogTime> GetIssueSpentTimesByDays(Issue issue, IEnumerable<DateTime> days)
        {
            var spentTimes = new List<TimeTrackLogTime>();
            foreach (var date in days)
            {
                var timeTrack = issue.TimeTracks.FirstOrDefault(t => t.TrackingDate == date);
                
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
