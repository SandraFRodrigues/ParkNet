using ParkNet.Entities.Infrastructure;

namespace ParkNet.Repositories.Contracts
{
    public interface IFloorRepository
    {
        Task<IEnumerable<Floor>> GetAllAsync();
    }
}
