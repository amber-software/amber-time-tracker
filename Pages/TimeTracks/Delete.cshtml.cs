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
    public class DeleteModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public DeleteModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeTrack = await _context.TimeTrack.FindAsync(id);

            if (TimeTrack != null)
            {
                _context.TimeTrack.Remove(TimeTrack);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
