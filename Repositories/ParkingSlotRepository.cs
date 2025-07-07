using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using ParkNet.Interfaces;

namespace ParkNet.Repositories
{
    public class ParkingSlotRepository : GenericRepository<ParkingSlot>, IParkingSlotRepository
    {
        public ParkingSlotRepository(ParkNetDbContext context) : base(context) { }

        public async Task<IEnumerable<ParkingSlot>> GetAvailableSlotsAsync()
        {
            return await _context.ParkingSlots
                .Where(ps => ps.EmptySpot)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingSlot>> GetSlotsByFloorIdAsync(int floorId)
        {
            return await _context.ParkingSlots
                .Where(ps => ps.FloorId == floorId)
                .ToListAsync();
        }
    }
}
