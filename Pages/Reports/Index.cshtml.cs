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
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages.Reports
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager,
                           ISprintsService sprintsService)
                                  : base(context, authorizationService, userManager, sprintsService)
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
            
            Sprint targetSprint = null;
            if (startDate.HasValue && stopDate.HasValue)
            {
                StartDate = startDate;
                StopDate = stopDate;
            }
            else
            {
                targetSprint = await sprintsService.GetTargetSprintOrCurrentSprint(id);
                StartDate = targetSprint.StartDate;
                StopDate = targetSprint.StopDate;
            }

            if (!StartDate.HasValue || !StopDate.HasValue)
                throw new ApplicationException("Can't create report: not enough parameters");            

            ReportDays = Enumerable.Range(0, 1 + StopDate.Value
                                                        .Subtract(StartDate.Value).Days)
                                                        .Select(offset => StartDate.Value.AddDays(offset))
                                                        .ToList();

            SpentTimes = context.Issue
                            .Include(i => i.TimeTracks)
                            .AsNoTracking()
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
            
            TargetSprintId = targetSprint?.ID;
            await PopulateSprintsDropDownList(TargetSprintId);
            return Page();
        }

        private async Task PopulateSprintsDropDownList(object selectedSprint = null)
        {            
            SprintsSL = new SelectList(await sprintsService.GetAllSprints(),
                        "ID", "SprintNumber", selectedSprint);
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
