using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{
    public class IndexModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public IndexModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public IList<TimeTrack> TimeTrack { get;set; }

        public async Task OnGetAsync()
        {
            TimeTrack = await _context.TimeTrack.ToListAsync();
        }
    }
}
