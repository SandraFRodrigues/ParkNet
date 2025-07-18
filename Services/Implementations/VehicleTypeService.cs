using ParkNet.Entities.Types;
using ParkNet.Services.Contracts;
using ParkNet.Repositories.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VehicleTypeService : IVehicleTypeService
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;

    public VehicleTypeService(IVehicleTypeRepository vehicleTypeRepository)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
    }

    public async Task<IEnumerable<VehicleType>> GetAllVehicleTypesAsync()
    {
        return await _vehicleTypeRepository.GetAllAsync();
    }
}
