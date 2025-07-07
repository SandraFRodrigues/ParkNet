using ParkNet.Entities.Infrastructure;

namespace ParkNet.Entities.Enums
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string Designation { get; set; }
        public string Code { get; set; }

        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    }
}
