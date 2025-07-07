using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Floors
{
    public class EditModel : PageModel
    {
        private readonly ParkNet.Data.ParkNetDbContext _context;

        public EditModel(ParkNet.Data.ParkNetDbContext context)
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

            var floor =  await _context.Floors.FirstOrDefaultAsync(m => m.Id == id);
            if (floor == null)
            {
                return NotFound();
            }
            Floor = floor;
           ViewData["BuildingId"] = new SelectList(_context.Buildings, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Floor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FloorExists(Floor.Id))
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

        private bool FloorExists(int id)
        {
            return _context.Floors.Any(e => e.Id == id);
        }
    }
}
