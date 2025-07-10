using ParkNet.Data;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Infrastructure;
using ParkNet.Helpers;
using ParkNet.Services.Contracts;



namespace ParkNet.Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly ParkNetDbContext _context;

        private readonly List<(string Code, int Id)> vehicleTypes = new()
        {
            new("C", 1), 
            new("M", 2)  
        };

        public ImportService(ParkNetDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBuildingFromLayoutAsync(string buildingName, string layout)

        {
            string[] lines = layout.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            var building = new Building
            {
                Name = buildingName,
                Floors = new List<Floor>()
            };

            int floorNumber = 0;
            int order = 0;
            Floor currentFloor = new Floor { Level = floorNumber, ParkingSlots = new List<ParkingSlot>() };

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentFloor.ParkingSlots.Any())
                    {
                        building.Floors.Add(currentFloor);
                        floorNumber++;
                        currentFloor = new Floor { Level = floorNumber, ParkingSlots = new List<ParkingSlot>() };
                        order = 0;
                    }
                    continue;
                }

                char[] chars = line.ToCharArray();
                foreach (var ch in chars)
                {
                    var slot = new ParkingSlot
                    {
                        Order = order,
                        Spot = CodeGenerator.GetLetterBasedOnNumber(floorNumber, currentFloor.ParkingSlots.Count + 1),
                        Status = SlotStatus.Free
                    };

                    if (!char.IsWhiteSpace(ch))
                    {
                        var vt = vehicleTypes.FirstOrDefault(x => x.Code.Equals(ch.ToString(), StringComparison.OrdinalIgnoreCase));
                        if (vt.Id > 0)
                            slot.VehicleTypeId = vt.Id;
                    }

                    currentFloor.ParkingSlots.Add(slot);
                    order++;
                }
            }

            if (currentFloor.ParkingSlots.Any())
                building.Floors.Add(currentFloor);

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
