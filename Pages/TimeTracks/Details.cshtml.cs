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
    public class DetailsModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public DetailsModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeTrack = await _context.TimeTrack.FirstOrDefaultAsync(m => m.ID == id);

            if (TimeTrack == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
