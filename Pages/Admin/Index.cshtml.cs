using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ParkNet.Pages.Admin
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public IndexModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public List<Building> Buildings { get; set; }

        public async Task OnGetAsync()
        {
            Buildings = await _context.Buildings
                .Include(b => b.Floors)
                .ToListAsync();
        }
    }
}