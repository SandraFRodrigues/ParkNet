using System.Collections.Generic;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Entities.Types
{
    public class VehicleType
    {
        public int Id { get; set; }

        public required string Designation { get; set; }

        public required string Code { get; set; }

        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    }
}
