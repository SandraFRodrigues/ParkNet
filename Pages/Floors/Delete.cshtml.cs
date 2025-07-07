using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Floors
{
    public class DeleteModel : PageModel
    {
        private readonly ParkNet.Data.ParkNetDbContext _context;

        public DeleteModel(ParkNet.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Floor Floor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors.FirstOrDefaultAsync(m => m.Id == id);

            if (floor is not null)
            {
                Floor = floor;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors.FindAsync(id);
            if (floor != null)
            {
                Floor = floor;
                _context.Floors.Remove(Floor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
