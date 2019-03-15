using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Services.Sprints
{
    public class SprintsService : ISprintsService
    {
        private TimeTracking.Models.TimeTrackDataContext _context;

        private IQueryable<Sprint> sprintsQuery => from sp in _context.Sprint.Include(s => s.Issues).ThenInclude(i => i.TimeTracks)
                                                        orderby sp.StartDate descending // Sort by name.
                                                        select sp;
        
        public SprintsService(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Sprint>> GetAllSprints()
        {
            return await sprintsQuery.AsNoTracking().ToListAsync();
        }

        public async Task<Sprint> GetTargetSprint(int? sprintId)
        {            
            return await sprintsQuery.AsNoTracking().FirstOrDefaultAsync(s => s.ID == sprintId);                                        
        }

        public async Task<Sprint> GetCurrentSprint()
        {
            var nowDate = DateTime.Now.Date;            
            return await sprintsQuery.AsNoTracking().FirstOrDefaultAsync(s => s.StartDate <= nowDate && nowDate < s.StopDate);
        }
    }
}