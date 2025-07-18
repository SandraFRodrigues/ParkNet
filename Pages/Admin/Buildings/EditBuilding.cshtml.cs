using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNet.Data;
using ParkNet.Entities;
using Microsoft.EntityFrameworkCore;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Admin
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class EditBuildingModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public EditBuildingModel(ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Building? Building { get; set; } 

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Building = await _context.Buildings.FindAsync(id);

            if (Building == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var buildingInDb = await _context.Buildings.FindAsync(Building.Id);

            if (buildingInDb == null)
                return NotFound();

            buildingInDb.Name = Building.Name;

            await _context.SaveChangesAsync();

            return RedirectToPage("ListBuildings");
        }
    }
}
