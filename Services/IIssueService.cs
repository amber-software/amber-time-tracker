using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTracking.Models;

namespace TimeTracking.Services.Issues
{
    public interface IIssueService
    {
        Task<IList<Issue>> GetAllIssues();


        Task<IList<Issue>> GetAllIssuesWithTheirSprints();
        Task<IList<Issue>> GetIssuesWithoutSprints();

        Task<Issue> GetTargetIssue(int? issueId);

        Task<Issue> GetIssueWithSprint(int? issueId);
    }
}