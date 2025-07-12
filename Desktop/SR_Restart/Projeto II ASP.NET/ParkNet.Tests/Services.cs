using Microsoft.Extensions.Logging;
using Moq;
using ParkNet.Data;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Types;
using ParkNet.Services.Contracts;
using ParkNet.Services.Implementations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace ParkNet.Tests.Services
{
    public class ImportServiceTests
    {
        private readonly Mock<IVehicleTypeService> _vehicleTypeServiceMock;
        private readonly Mock<ILogger<IBuildingImportService>> _loggerMock;
        private readonly ParkNetDbContext _context;

        public ImportServiceTests()
        {
            var options = new DbContextOptionsBuilder<ParkNetDbContext>()
                .UseInMemoryDatabase(databaseName: "ParkNetTestDb")
                .Options;

            _context = new ParkNetDbContext(options);
            _vehicleTypeServiceMock = new Mock<IVehicleTypeService>();
            _loggerMock = new Mock<ILogger<IBuildingImportService>>();

            // MOCK CORRETO! Async!
            _vehicleTypeServiceMock.Setup(v => v.GetAllVehicleTypesAsync())
                .ReturnsAsync(new List<VehicleType>
                {
                    new VehicleType { Id = 1, Code = "C", Designation = "Car" },
                    new VehicleType { Id = 2, Code = "M", Designation = "Moto" }
                });
        }

        [Fact]
        public async Task CreateBuildingFromLayoutAsync_ShouldCreateBuilding_WhenLayoutIsValid()
        {
            var service = new ImportService(_context, _vehicleTypeServiceMock.Object, _loggerMock.Object);

            string buildingName = "Edificio Teste";
            string layout = "C M\nC C\n\nM M";

            bool result = await service.CreateBuildingFromLayoutAsync(buildingName, layout);

            Assert.True(result);
            var building = await _context.Buildings
                .Include(b => b.Floors).ThenInclude(f => f.ParkingSlots)
                .FirstOrDefaultAsync(b => b.Name == buildingName);

            Assert.NotNull(building);
            Assert.Equal(2, building.Floors.Count);
        }

        [Fact]
        public async Task CreateBuildingFromLayoutAsync_ShouldReturnFalse_WhenNameIsEmpty()
        {
            var service = new ImportService(_context, _vehicleTypeServiceMock.Object, _loggerMock.Object);

            var result = await service.CreateBuildingFromLayoutAsync("", "C M");

            Assert.False(result);
        }

        [Fact]
        public async Task CreateBuildingFromLayoutAsync_ShouldReturnFalse_WhenBuildingAlreadyExists()
        {
            string name = "Edificio Existente";
            _context.Buildings.Add(new Building { Name = name });
            await _context.SaveChangesAsync();

            var service = new ImportService(_context, _vehicleTypeServiceMock.Object, _loggerMock.Object);

            var result = await service.CreateBuildingFromLayoutAsync(name, "C M");

            Assert.False(result);
        }

        [Fact]
        public async Task CreateBuildingFromLayoutAsync_ShouldAssignCorrectVehicleTypeId()
        {
            var service = new ImportService(_context, _vehicleTypeServiceMock.Object, _loggerMock.Object);
            string layout = "C M";

            await service.CreateBuildingFromLayoutAsync("Teste Veiculos", layout);

            var slot = await _context.ParkingSlots.FirstOrDefaultAsync();

            Assert.NotNull(slot);
            Assert.Equal(1, slot.VehicleTypeId);
        }
    }
}
