using ParkNet.Data;
using Microsoft.EntityFrameworkCore;
using ParkNet.Entities.Infrastructure;
using ParkNet.Repositories.Contracts;

namespace ParkNet.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly ParkNetDbContext _context;

        public FloorRepository(ParkNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Floor>> GetAllAsync()
            => await _context.Floors.ToListAsync();

        public async Task<Floor?> GetByIdAsync(int id)
            => await _context.Floors.FindAsync(id);

        public async Task<Floor> AddAsync(Floor floor)
        {
            _context.Floors.Add(floor);
            await _context.SaveChangesAsync();
            return floor;
        }

        public async Task<bool> UpdateAsync(Floor floor)
        {
            _context.Floors.Update(floor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null) return false;
            _context.Floors.Remove(floor);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}