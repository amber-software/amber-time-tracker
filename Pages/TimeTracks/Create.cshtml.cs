using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{
    public class CreateModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public CreateModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TimeTrack.Add(TimeTrack);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}