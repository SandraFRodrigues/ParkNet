using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace ParkNet.Pages.Admin
{
    public class ViewSlotsModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public ViewSlotsModel(ParkNetDbContext context)
        {
            _context = context;
        }

        public List<Floor> Floors { get; set; } = new();
        public string BuildingName { get; set; } = "";

        public void OnGet(int buildingId)
        {
            Floors = _context.Floors
                .Where(f => f.BuildingId == buildingId)
                .Include(f => f.ParkingSlots)
                .OrderBy(f => f.Level)
                .ToList();

            var building = _context.Buildings.FirstOrDefault(b => b.Id == buildingId);
            BuildingName = building != null ? building.Name : "Edifício";
        }
    }
}
