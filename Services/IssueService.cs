using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Services.Issues
{
    public class IssueService : IIssueService
    {
        private TimeTracking.Models.TimeTrackDataContext _context;

        private IQueryable<Issue> issuesQuery => from sp in _context.Issue.Include(i => i.TimeTracks)                                                        
                                                        select sp;
        
        public IssueService(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Issue>> GetAllIssues()
        {
            return await issuesQuery.AsNoTracking().ToListAsync();
        }

        public async Task<IList<Issue>> GetAllIssuesWithSprints()
        {
            return await issuesQuery.Include(i => i.Sprint).Where(i => i.SprintID != null).AsNoTracking().ToListAsync();
        }     

        public async Task<IList<Issue>> GetIssuesWithoutSprints()
        {
            return await issuesQuery.Where(i => i.SprintID == null).AsNoTracking().ToListAsync();
        }

        public async Task<Issue> GetTargetIssue(int? issueId)
        {            
            return await issuesQuery.AsNoTracking().FirstOrDefaultAsync(s => s.ID == issueId);                                        
        }

        public async Task<Issue> GetIssueWithSprint(int? issueId)
        {            
            return await issuesQuery.Include(i => i.Sprint).AsNoTracking().FirstOrDefaultAsync(s => s.ID == issueId);
        }
    }
}