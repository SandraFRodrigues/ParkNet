using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities;

namespace ParkNet.Pages.Buildings
{
    public class EditModel : PageModel
    {
        private readonly ParkNet.Data.ParkNetDbContext _context;

        public EditModel(ParkNet.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Building Building { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building =  await _context.Buildings.FirstOrDefaultAsync(m => m.Id == id);
            if (building == null)
            {
                return NotFound();
            }
            Building = building;
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

            _context.Attach(Building).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(Building.Id))
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

        private bool BuildingExists(int id)
        {
            return _context.Buildings.Any(e => e.Id == id);
        }
    }
}
