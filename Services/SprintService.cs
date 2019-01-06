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

        public async Task<Sprint> GetTargetSprintOrCurrentSprint(int? sprintId)
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