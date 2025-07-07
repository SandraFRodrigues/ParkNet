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
    public class DetailsModel : PageModel
    {
        private readonly ParkNet.Data.ParkNetDbContext _context;

        public DetailsModel(ParkNet.Data.ParkNetDbContext context)
        {
            _context = context;
        }

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
    }
}
