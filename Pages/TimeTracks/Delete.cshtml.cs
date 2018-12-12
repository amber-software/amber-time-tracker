using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Pages.TimeTracks
{
    public class DeleteModel : TimeTrackModelBase
    {        
        public DeleteModel(TimeTracking.Models.TimeTrackDataContext context,
                           IAuthorizationService authorizationService,
                           UserManager<IdentityUser> userManager) 
                           : base(context, authorizationService, userManager)
        {            
        }

        [BindProperty]
        public TimeTrack TimeTrack { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeTrack = await context.TimeTrack.FirstOrDefaultAsync(m => m.ID == id);

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

            TimeTrack = await context.TimeTrack.FindAsync(id);

            if (TimeTrack != null)
            {
                context.TimeTrack.Remove(TimeTrack);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
