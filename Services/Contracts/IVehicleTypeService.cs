using ParkNet.Entities.Types;
using ParkNet.Entities.Infrastructure;
using System.Collections.Generic;

namespace ParkNet.Services.Contracts
{
    public interface IVehicleTypeService
    {
        Task<IEnumerable<VehicleType>> GetAllVehicleTypesAsync();
    }
}
