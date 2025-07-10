using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using System.Threading.Tasks;

namespace ParkNet.Pages.Admin
{
    public class ViewBuildingModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public ViewBuildingModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public Building? Building { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Building = await _context.Buildings
                .Include(b => b.Floors)
                    .ThenInclude(f => f.ParkingSlots)
                        .ThenInclude(ps => ps.VehicleType)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (Building == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
