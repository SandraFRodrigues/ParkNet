using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using ParkNet.Repositories.Contracts;

namespace ParkNet.Repositories
{
    public class ParkingSlotRepository : GenericRepository<ParkingSlot>, IParkingSlotRepository
    {
        private readonly ParkNetDbContext _dbContext;
        public ParkingSlotRepository(ParkNetDbContext context) : base(context)
        { 
            _dbContext = context;
        }


        public async Task<IEnumerable<ParkingSlot>> GetAvailableSlotsAsync()
        {
            return await _dbContext.ParkingSlots
                .Where(ps => ps.EmptySpot)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingSlot>> GetSlotsByFloorIdAsync(int floorId)
        {
            return await _dbContext.ParkingSlots
                .Where(ps => ps.FloorId == floorId)
                .ToListAsync();
        }
    }
}
