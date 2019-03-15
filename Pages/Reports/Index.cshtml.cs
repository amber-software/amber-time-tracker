using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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

        public SelectList PlatformsSL { get; set; }

        public int? TargetSprintId { get; set; }

        public Platform? TargetPlatform { get; set; }

        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To")]
        public DateTime? StopDate { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, DateTime? startDate, DateTime? stopDate, Platform? platform)
        {
            if (!(await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      ReportOperations.ViewReport))
                .Succeeded)
            {
                return new ChallengeResult();
            }
            
            await PrepareModel(id, startDate, stopDate, platform);
            
            TargetSprintId = id;
            TargetPlatform = platform;
            await PopulateSprintsDropDownList(id);
            PopulatePlatformsDropDownList(platform);
            return Page();
        }
        
        public async Task<IActionResult> OnGetCSVReportAsync(int? id, DateTime? startDate, DateTime? stopDate, Platform? platform)
        {
            if (!(await authorizationService.AuthorizeAsync(
                                                      User, new TimeTrack(),
                                                      ReportOperations.ViewReport))
                .Succeeded)
            {
                return new ChallengeResult();
            }
            
            await PrepareModel(id, startDate, stopDate, platform);

            var reportcsv = new StringBuilder();

            var comlumnsHeadrs = new List<string>()
            {
                "Task Number",
                "Description",
                "Priority",
                "Estimation",
                "RemainingTime",
                "Status"
            };

            foreach (var day in ReportDays)
                comlumnsHeadrs.Add($"{day.ToString("ddd")} {day.Date.Day}");

            comlumnsHeadrs.Add("Total");

            float totalHours = 0f;
            foreach (var item in SpentTimes)
            {
                reportcsv.Append(item.IssueNumber).Append(',');
                reportcsv.Append(item.IssueDescription).Append(',');
                reportcsv.Append(item.Priority).Append(',');
                reportcsv.Append(item.Estimate).Append(',');
                reportcsv.Append(item.RemainingTime).Append(',');
                reportcsv.Append(item.Status).Append(',');

                float totalTaskHours = 0f;
                foreach (var loggedTime in item.LoggedTimes)
                {                
                    totalTaskHours += loggedTime.Hours ?? 0;
                    reportcsv.Append(loggedTime.Hours).Append(',');
                }

                reportcsv.Append(totalTaskHours).AppendLine();

                totalHours += totalTaskHours;        
            }

            reportcsv.Append(platform.HasValue ? "Platform" : string.Empty).Append(',');
            reportcsv.Append(platform.HasValue ? platform.Value.ToString() : string.Empty).Append(',');
            
            reportcsv.Append("Total").Append(',');
            reportcsv.Append(SpentTimes.Sum(t => t.Estimate)).Append(',');
            reportcsv.Append(SpentTimes.Sum(t => t.RemainingTime)).Append(',');
            reportcsv.Append(',');

            for (var i = 0; i < ReportDays.Count; i++)
            {            
                reportcsv.Append(SpentTimes.Sum(t => t.LoggedTimes[i].Hours ?? 0)).Append(',');
            }

            reportcsv.Append(totalHours);
            
            byte[] buffer = Encoding.ASCII.GetBytes($"{string.Join(",", comlumnsHeadrs)}\r\n{reportcsv.ToString()}");
            
            var platformStr = platform.HasValue ? $"{platform.Value}-" : string.Empty;

            return File(buffer, "text/csv", $"report_{platformStr}{StartDate.Value.ToString("yyyy-MM-dd")}-{StopDate.Value.ToString("yyyy-MM-dd")}.csv");
        }

        private async Task PopulateSprintsDropDownList(object selectedSprintID = null)
        {
            var selectValues = (await sprintsService.GetAllSprints())
                                .Select(s => new { ID = s.ID, Text = s.SprintNumber })
                                .ToList();
            selectValues.Add(new { ID = -1, Text = "Without Sprint" });

            SprintsSL = new SelectList(selectValues, "ID", "Text", selectedSprintID);            
        }

        private void PopulatePlatformsDropDownList(Platform? selectedPlatform = null)
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

        private async Task PrepareModel(int? id, DateTime? startDate, DateTime? stopDate, Platform? platform)
        {
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

            // filter issues has timetracks in selected date range
            var filteredIssues = issues
                                .Where(i => !platform.HasValue || i.Platform == platform.Value) // filter issues by platform if specified
                                .Where(i => i.TimeTracks.Any(t => t.TrackingDate >= StartDate.Value && t.TrackingDate < StopDate.Value)); // filter issues by tracks dates
            
            SpentTimes = filteredIssues
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
