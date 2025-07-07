using ParkNet.Data;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Operations;

namespace ParkNet.Services.BuildingImportServices
{
    public class BuildingImportServices
    {
        private readonly ParkNetDbContext _context;

        private static readonly string _alphabet = new string(
            Enumerable.Range('A', 26).Select(c => (char)c)
            .Concat(Enumerable.Range('A', 26).Select(c => (char)c)).ToArray());

        private readonly List<VehicleType> _vehicleTypes;

        public BuildingImportServices(ParkNetDbContext context)
        {
            _context = context;
            _vehicleTypes = _context.VehicleTypes.ToList();
        }

        public async Task<Building> ImportFromTextAsync(string importedText)
        {
            if (string.IsNullOrWhiteSpace(importedText))
                return null;

            var lines = importedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Length == 0 || lines.All(string.IsNullOrWhiteSpace))
                return null;

            var building = new Building
            {
                Name = $"Edifício {DateTime.Now:yyyyMMddHHmmssffff}",
                Floors = new List<Floor>()
            };

            int floorIndex = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    floorIndex++;
                    continue;
                }

                var floor = new Floor
                {
                    Name = $"Piso {floorIndex}",
                    ParkingSlots = new List<ParkingSlot>()
                };

                int order = 0;

                foreach (char slotChar in line)
                {
                    var parkingSlot = new ParkingSlot
                    {
                        Order = order++,
                        Spot = string.Empty
                    };

                    var matchedVehicle = _vehicleTypes.FirstOrDefault(v =>
                        v.Code.Equals(slotChar.ToString(), StringComparison.OrdinalIgnoreCase));

                    if (matchedVehicle != null)
                    {
                        parkingSlot.Spot = GetLetter(floorIndex, floor.ParkingSlots.Count + 1);
                        parkingSlot.VehicleTypeId = matchedVehicle.Id;
                    }

                    floor.ParkingSlots.Add(parkingSlot);
                }

                building.Floors.Add(floor);
                floorIndex++;
            }

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            return building;
        }

        private string GetLetter(int floor, int spot)
        {
            if (floor >= _alphabet.Length || spot >= _alphabet.Length)
                throw new Exception("Overflow de letras! Excede limite de codificação.");

            string suffix = spot > 26 ? GetPos(spot) : spot.ToString();
            return $"{GetPos(floor)}{suffix}";
        }

        private string GetPos(int pos)
        {
            return _alphabet[pos].ToString();
        }
    }
}
