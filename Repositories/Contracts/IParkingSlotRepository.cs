using ParkNet.Entities.Infrastructure;

namespace ParkNet.Repositories.Contracts
{
    public interface IParkingSlotRepository : IGenericRepository<ParkingSlot>
    {
        Task<IEnumerable<ParkingSlot>> GetAvailableSlotsAsync();
        Task<IEnumerable<ParkingSlot>> GetSlotsByFloorIdAsync(int floorId);

    }
}
