using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Entities;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Admin
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class ListBuildingsModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public ListBuildingsModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public List<Building> Buildings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Buildings = await _context.Buildings
                .Include(b => b.Floors)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var building = await _context.Buildings
                .Include(b => b.Floors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (building == null)
            {
                return NotFound();
            }
          
            if (building.Floors != null && building.Floors.Any())
            {
                _context.Floors.RemoveRange(building.Floors);
            }

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
