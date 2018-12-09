using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.BusinessConstants
{
    public class DetailsModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public DetailsModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        public BusinessConstant BusinessConstant { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BusinessConstant = await _context.BusinessConstant.FirstOrDefaultAsync(m => m.ID == id);

            if (BusinessConstant == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
