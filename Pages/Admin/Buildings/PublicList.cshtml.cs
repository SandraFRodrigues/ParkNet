using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Buildings
{
    public class PublicListModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public PublicListModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public List<Building> Buildings { get; set; } = new();

        public async Task OnGetAsync()
        {
            Buildings = await _context.Buildings
                .Include(b => b.Floors)
                .ToListAsync();
        }
    }
}
