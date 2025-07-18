using ParkNet.Data;
using ParkNet.Entities.Types;
using ParkNet.Entities.Infrastructure;
using ParkNet.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ParkNet.Services.Implementations
{
    public class BuildingImportService : IBuildingImportService
    {
        private readonly ParkNetDbContext _context;

        public BuildingImportService(ParkNetDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ImportFromTextAsync(string importedText, string buildingName)
        {
            if (string.IsNullOrWhiteSpace(importedText))
                return false;

            var pisos = importedText
                .Split(new[] { "\r\n\r\n", "\n\n", "\r\r" }, StringSplitOptions.RemoveEmptyEntries);

            if (pisos.Length == 0)
                return false;

            var vehicleTypes = await _context.VehicleTypes.ToListAsync();
            if (vehicleTypes.Count == 0)
                throw new InvalidOperationException("Não existem tipos de veículo! Faz seed antes de importar.");

            var building = new Building
            {
                Name = string.IsNullOrWhiteSpace(buildingName)
                    ? $"Edifício {DateTime.Now:yyyyMMddHHmmssffff}"
                    : buildingName,
                Floors = new List<Floor>()
            };

            int floorIndex = 0;
            int globalOrder = 0;

            foreach (var pisoText in pisos)
            {
                var lines = pisoText
                    .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                var floor = new Floor
                {
                    Name = $"Piso {floorIndex + 1}",
                    Level = floorIndex + 1,
                    BuildingId = building.Id,
                    ParkingSlots = new List<ParkingSlot>()
                };

                for (int row = 0; row < lines.Length; row++)
                {
                    var line = lines[row];
                    string letraLinha = GetSpotRowLetter(row);

                    for (int col = 0; col < line.Length; col++)
                    {
                        char slotChar = line[col];

                        if (char.IsWhiteSpace(slotChar))
                            continue;

                        var matchedVehicle = vehicleTypes.FirstOrDefault(v =>
                            v.Code.Equals(slotChar.ToString(), StringComparison.OrdinalIgnoreCase));

                        if (matchedVehicle != null)
                        {
                            string spot = $"{letraLinha}{col + 1}";
                            var parkingSlot = new ParkingSlot
                            {
                                Order = globalOrder++,
                                Spot = spot,
                                VehicleTypeId = matchedVehicle.Id
                            };
                            floor.ParkingSlots.Add(parkingSlot);
                        }
                    }
                }
                building.Floors.Add(floor);
                floorIndex++;
            }

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            return true;
        }

        private string GetSpotRowLetter(int rowIndex)
        {
            string result = "";
            rowIndex++;
            while (rowIndex > 0)
            {
                rowIndex--;
                result = (char)('A' + (rowIndex % 26)) + result;
                rowIndex /= 26;
            }
            return result;
        }
    }
}


