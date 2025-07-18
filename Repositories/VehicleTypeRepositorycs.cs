using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Types;
using ParkNet.Repositories.Contracts;

namespace ParkNet.Repositories
{
    public class VehicleTypeRepository : GenericRepository<VehicleType>, IVehicleTypeRepository
    {
        public VehicleTypeRepository(ParkNetDbContext context) : base(context) { }

    }
}