using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Pages.Buildings
{
    public class IndexModel : PageModel
    {
        private readonly ParkNet.Data.ParkNetDbContext _context;

        public IndexModel(ParkNet.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public IList<Building> Building { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Building = await _context.Buildings.ToListAsync();
        }
    }
}
