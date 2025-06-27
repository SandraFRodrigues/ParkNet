using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities;

namespace ParkNet.Pages.Floors
{
    public class IndexModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public IndexModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public IList<Floor> Floors { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Floors = await _context.Floors.Include(f => f.Building).ToListAsync();
        }
    }
}