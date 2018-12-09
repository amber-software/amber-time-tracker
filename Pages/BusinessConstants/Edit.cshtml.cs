using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.BusinessConstants
{
    public class EditModel : PageModel
    {
        private readonly TimeTracking.Models.TimeTrackDataContext _context;

        public EditModel(TimeTracking.Models.TimeTrackDataContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(BusinessConstant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessConstantExists(BusinessConstant.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BusinessConstantExists(int id)
        {
            return _context.BusinessConstant.Any(e => e.ID == id);
        }
    }
}
